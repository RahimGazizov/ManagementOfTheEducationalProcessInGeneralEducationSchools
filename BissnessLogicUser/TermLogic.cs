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
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EditTerm(Term term)
        {
            var exists = await _context.Term
                .AnyAsync(t => t.Name.Trim().ToLower() == term.Name.Trim().ToLower()
                && t.AcademicYearId == term.AcademicYearId && t.Id != term.Id);
            if (exists)
                return OperationResult.Fail("Такая запись уже есть");
            string hasError = await CheckData(term);
            if (!string.IsNullOrWhiteSpace(hasError))
                return OperationResult.Fail(hasError);
            return OperationResult.Ok();
        }
        private async Task<string?> CheckData(Term term)
        {
            var academicYear = await _context.AcademicYear
     .FirstOrDefaultAsync(a => a.Id == term.AcademicYearId);
            string[] name = term.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (name[0].StartsWith('0'))
                return "Номер четверти нельзя писать с ведущим нулём";
            if (name.Length > 2 || name.Length < 2)
                return "Введите в формате: 1 четверть";
            if (!int.TryParse(name[0], out int value))
                return "Введите номер четверти целым числовым форматом";
            int currentValue = value;
            if (currentValue > 1)
            {
                bool hasPreviousQuarter = await _context.Term
                    .AnyAsync(t => t.QuarterNumber == currentValue - 1);
                if (!hasPreviousQuarter)
                    return $"Сначала добавьте {currentValue - 1} четрветь";
            }
            if (value <= 0 || value > 4)
                return "Введите номер четверти целым числовым форматом от 1 до 4";
            if (name[1].ToString().ToLower() != "четверть")
                return "Введите слово четверть правильно";
            if (academicYear != null)
            {
                if (academicYear.Terms.Count >= 4)
                    return "В этом учебном году уже добавлены все четверти";
            }
            var lastDate = await _context.Term
                .Where(a => a.AcademicYearId == term.AcademicYearId)
                .OrderBy(a => a.QuarterNumber)
                .Select(d => (DateTime?)d.DateEndTerm)
                .LastOrDefaultAsync();
            if (lastDate != null && lastDate > term.DateStartTerm)
                return "Дата начала новой четверти должна быть позже предыдущей четверти";
            term.QuarterNumber = value;
            _context.Term.Add(term);
            await _context.SaveChangesAsync();
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
