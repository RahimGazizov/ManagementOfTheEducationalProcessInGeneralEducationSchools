using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class TermLogic
    {
        private readonly AppDbContext _context;
        public TermLogic(AppDbContext context)
        {
            _context = context;
        }
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public static OperationResult Ok() => new OperationResult { Success = true };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        public async Task<OperationResult> AddTerm(Term term)
        {
            var exists = await _context.Term
                .AnyAsync(t => t.Name.Trim().ToLower() == term.Name.Trim().ToLower()
                && t.AcademicYearId == term.AcademicYearId);
            if (exists)
                return OperationResult.Fail("Такая запись уже есть");
            string hasError = await CheckData(term);
            if (!string.IsNullOrWhiteSpace(hasError))
                return OperationResult.Fail(hasError);
            _context.Term.Add(term);
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EditTerm(Term term)
        {
            var current = await _context.Term.FindAsync(term.Id);

            if (current == null)
                return OperationResult.Fail("Запись не найдена");

            var exists = await _context.Term.AnyAsync(t =>
                t.Name.Trim().ToLower() == term.Name.Trim().ToLower() &&
                t.AcademicYearId == term.AcademicYearId &&
                t.Id != term.Id);

            if (exists)
                return OperationResult.Fail("Такая запись уже есть");

            string hasError = await CheckData(term);
            if (!string.IsNullOrWhiteSpace(hasError))
                return OperationResult.Fail(hasError);

            current.QuarterNumber = term.QuarterNumber;
            current.DateStartTerm = term.DateStartTerm;
            current.DateEndTerm = term.DateEndTerm;
            current.Name = term.Name;
            current.AcademicYearId = term.AcademicYearId;

            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }
        private async Task<string?> CheckData(Term term)
        {
            string[] name = term.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (name.Length != 2)
                return "Введите в формате: 1 четверть";

            if (name[0].StartsWith('0'))
                return "Номер четверти нельзя писать с ведущим нулём";

            if (!int.TryParse(name[0], out int value))
                return "Введите номер четверти числом";

            if (value <= 0 || value > 4)
                return "Номер четверти от 1 до 4";

            if (name[1].ToLower() != "четверть")
                return "Введите слово 'четверть' правильно";

            // 🔥 предыдущая четверть (правильно)
            if (value > 1)
            {
                bool hasPrevious = await _context.Term.AnyAsync(t =>
                    t.AcademicYearId == term.AcademicYearId &&
                    t.QuarterNumber == value - 1 &&
                    t.Id != term.Id);

                if (!hasPrevious)
                    return $"Сначала добавьте {value - 1} четверть";
            }

            // 🔥 максимум 4 четверти (правильно через DB)
            var count = await _context.Term.CountAsync(t =>
                t.AcademicYearId == term.AcademicYearId &&
                t.Id != term.Id);

            if (count >= 4)
                return "В этом учебном году уже 4 четверти";

            // 🔥 проверка дат
            if (term.DateStartTerm >= term.DateEndTerm)
                return "Дата начала должна быть меньше даты окончания";

            // 🔥 пересечение с другими четвертями
            bool overlap = await _context.Term.AnyAsync(t =>
                t.AcademicYearId == term.AcademicYearId &&
                t.Id != term.Id &&
                term.DateStartTerm <= t.DateEndTerm &&
                term.DateEndTerm >= t.DateStartTerm);

            if (overlap)
                return "Даты пересекаются с другой четвертью";

            return null;
        }
        public List<SelectListItem> ListAcademicYear()
        {
            var list = _context.AcademicYear.Select(a => new SelectListItem
            {
                Value = a.Id,
                Text = a.Name
            }).ToList();
            return list;
        }
    }
}
