using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class TermLogic
    {
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionLogService;
        public TermLogic(AppDbContext context, ActionLogService actionLogService)
        {
            _context = context;
            _actionLogService = actionLogService;
        }
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public static OperationResult Ok() => new OperationResult { Success = true };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        private class TermDto
        {
            public string Name { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string AcademicYearName { get; set; }
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
            var num = term.Name.Split(' ');
            term.QuarterNumber = Convert.ToInt32(num[0]);
            _context.Term.Add(term);
            await _context.SaveChangesAsync();
            var termForLog = await GetTerm(term.Id);
            if (termForLog == null)
                return OperationResult.Fail("Четверть не найдена для лога");
            await _actionLogService.LogAsync(
                "Добавление четверти",
                "Четверть",
                term.Id,
                $"Добавлена четверть: Название: {termForLog.Name}. Дата начало: {termForLog.StartDate.ToString("dd-MM-yyyy")}." +
                $" Дата конца: {termForLog.EndDate.ToString("dd-MM-yyyy")} Учебный год: {termForLog.AcademicYearName}."
                );
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
            var oldTerm = await GetTerm(current.Id);
            if (oldTerm == null)
                return OperationResult.Fail("Четверть не найдена для лога");
            current.QuarterNumber = term.QuarterNumber;
            current.DateStartTerm = term.DateStartTerm;
            current.DateEndTerm = term.DateEndTerm;
            current.Name = term.Name;
            current.AcademicYearId = term.AcademicYearId;
            var num = term.Name.Split(' ');
            current.QuarterNumber = Convert.ToInt32(num[0]);
            await _context.SaveChangesAsync();
            var newTerm = await GetTerm(term.Id);
            if (newTerm == null)
                return OperationResult.Fail("Четверть не найдена для лога");
            await _actionLogService.LogAsync(
              "Редактирование четверти",
              "Четверть",
              term.Id,
              $"Редактирована четверть: Название: {oldTerm.Name} → {newTerm.Name}. Дата начало: " +
              $"{oldTerm.StartDate.ToString("dd-MM-yyyy")} → {newTerm.StartDate.ToString("dd-MM-yyyy")}." +
              $" Дата конца: {oldTerm.EndDate.ToString("dd-MM-yyyy")} → {newTerm.EndDate.ToString("dd-MM-yyyy")}" +
              $" Учебный год: {oldTerm.AcademicYearName} → {newTerm.AcademicYearName}."
              );
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
        public async Task<OperationResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult.Fail("Айди пуст удаление не возможно");
            var term = await _context.Term.FirstOrDefaultAsync(s => s.Id == id);
            if (term == null)
                return OperationResult.Fail("Четверть не найдена для лога, удаление не возможно");
            var termForLog = await GetTerm(term.Id);
            _context.Term.Remove(term);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
    "Удаление четверти",
    "Четверть",
    id,
    $"Удалена четверть: Название: {termForLog.Name}. Дата начало: {termForLog.StartDate.ToString("dd-MM-yyyy")}." +
    $" Дата конца: {termForLog.EndDate.ToString("dd-MM-yyyy")} Учебный год: {termForLog.AcademicYearName}."
    );
            return OperationResult.Ok();
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
        private async Task<TermDto> GetTerm(string id)
        {
            return await _context.Term
                .Where(s => s.Id == id)
                .Select(s => new TermDto
                {
                    Name = s.Name,
                    StartDate = s.DateStartTerm,
                    EndDate = s.DateEndTerm,
                    AcademicYearName = s.AcademicYear.Name,
                })
                .FirstOrDefaultAsync();
        }
    }
}
