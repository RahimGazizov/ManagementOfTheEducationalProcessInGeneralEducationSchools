using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;

namespace InformationSystemOfASchoolIducationalPortal.Service
{
    public class JournalService
    {
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionService;
        public JournalService(AppDbContext context, ActionLogService actionService)
        {
            _context = context;
            _actionService = actionService;
        }
        public class OperationResult<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
            public static OperationResult<T> Ok(T? data, string? message) => new OperationResult<T> { Success = true, Data = data, Message = message };
            public static OperationResult<T> Fail(string message) => new OperationResult<T> { Success = false, Message = message };
        }
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public static OperationResult Ok() => new OperationResult { Success = true };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        private class JournalEntryDto
        {
            public string StudentName { get; set; }
            public string Present { get; set; }
            public int? Grade { get; set; }
            public string SubjectName { get; set; }

        }
        private class JournalDto
        {
            public string LessonTopic { get; set; }
            public string HomeWork { get; set; }
            public string CLass { get; set; }
        }
        public async Task<OperationResult<string>> CreateJournal(string teacherId, string subjectId, string classId)
        {
            var dayToday = DateTime.Today;
            var currentYear = await GetAcademicYear(dayToday);
            if (currentYear == null)
                return OperationResult<string>.Fail("Не найден текущий учебный год");
            var currentTerm = await GetTerm(dayToday, currentYear.Id);
            if (currentTerm == null)
                return OperationResult<string>.Fail("Не найдена текущая четверть");
            var teacher = await _context.Teachers
             .Where(s => s.Id == teacherId)
             .Select(s => s.User.FullName).FirstOrDefaultAsync();
            if (teacher == null)
                return OperationResult<string>.Fail("Учитель не найден");
            var subName = await _context.Subjects
                .Where(s => s.Id == subjectId)
                .Select(s => s.Name).FirstOrDefaultAsync();
            if (subName == null)
                return OperationResult<string>.Fail("Предмет не найден");
            var className = await _context.Classes
              .Where(s => s.Id == classId)
              .Select(s => new
              {
                  classNum = s.NumClass,
                  classLett = s.LetterClass
              }).FirstOrDefaultAsync();
            if (className == null)
                return OperationResult<string>.Fail("Класс не найден");

            var journal = new Journal
            {
                Date = dayToday,
                TeacherId = teacherId,
                SubjectId = subjectId,
                ClassId = classId,
                AcademicYearId = currentYear.Id,
                TermId = currentTerm.Id,
                HomeWork = null,
                LessonTopic = null,
                IsLocked = true
            };

            var students = await _context.Students.Where(s => s.ClassId == classId).ToListAsync();
            foreach (var student in students)
            {
                var journalEntri = new JournalEntry
                {
                    StudentId = student.Id,
                    Grade = null
                };
                journal.Entries.Add(journalEntri);
            }
            _context.Journal.Add(journal);
            await _context.SaveChangesAsync();
         
            await _actionService.LogAsync(
                "Создание журнала",
                "Журнал",
                journal.Id,
                $"Учитель создал журнал: Имя учителя: {teacher}. Предмет: {subName}. Класс: {className.classNum}-{className.classLett}."
                );
            return OperationResult<string>.Ok(journal.Id, null);
        }
        public async Task<OperationResult> Delete(string id)
        {
           
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult.Fail("Айди журнала пуст, удаление невозможно");
            var journal = await _context.Journal.Include(j => j.Entries)
    .Where(j => j.Id == id)
    .FirstOrDefaultAsync();
            if (journal == null)
                return OperationResult.Fail("Журнал не найден, удаление невозможно");
            var journalForLog = await _context.Journal
               .Where(s => s.Id == id)
               .Select(s => new
               {
                   TeacherName = s.Teacher.User.FullName,
                   SubName = s.Subject.Name,
                   ClassNum = s.Class.NumClass,
                   ClassLett = s.Class.LetterClass
               }).FirstOrDefaultAsync();
            _context.Remove(journal);
            await _context.SaveChangesAsync();
            await _actionService.LogAsync(
                "Удаление журнала",
                "Журнал",
                journal.Id,
                $"Учитель удалил журнал: Имя учителя: {journalForLog.TeacherName}. Предмет: {journalForLog.SubName}. " +
                $"Класс: {journalForLog.ClassNum}-{journalForLog.ClassLett}."
                );
            return OperationResult.Ok();
        }
        public async Task<OperationResult<string>> SaveJournal(Journal journal)
        {
           
            var exists = await _context.Journal
                .Include(s => s.Subject)
                .Include(s => s.Class)
                .Include(s => s.Entries)
                .FirstOrDefaultAsync(j => j.Id == journal.Id);

            if (exists == null)
                return OperationResult<string>.Fail("Журнал не найден");
            if (!exists.IsLocked)
                return OperationResult<string>.Fail("Редактирование не возможно, истек срок возможности редактирование");
            // фикс темы и дз
            bool lessonChanged =
                exists.LessonTopic != journal.LessonTopic ||
                exists.HomeWork != journal.HomeWork;

            var changes = new List<string>();

            if (lessonChanged)
            {
                changes.Add($"Тема: {exists.LessonTopic} → {journal.LessonTopic}");
                changes.Add($"ДЗ: {exists.HomeWork} → {journal.HomeWork}");
            }

            exists.LessonTopic = journal.LessonTopic;
            exists.HomeWork = journal.HomeWork;

            var entryChanges = new List<string>();

            foreach (var entryModal in journal.Entries)
            {
                var entry = await _context.JournalEntry
                    .Include(s => s.Student)
                    .ThenInclude(s => s.User)
                    .FirstOrDefaultAsync(e => e.Id == entryModal.Id);

                if (entry == null)
                    continue;

                // сохраняем старые значения
                var oldGrade = entry.Grade;
                var oldPresent = entry.IsPresent;

                // обновляем
                entry.IsPresent = entryModal.IsPresent;
                entry.Grade = entryModal.Grade;

                // логируем ТОЛЬКО изменения
                if (oldGrade != entry.Grade || oldPresent != entry.IsPresent)
                {
                    entryChanges.Add(
                        $"{entry.Student.User.FullName}: " +
                        $"{(oldPresent ? "присутствовал" : "отсутствовал")} → {(entry.IsPresent ? "присутствовал" : "отсутствовал")}, " +
                        $"оценка: {(oldGrade?.ToString() ?? "нет")} → {(entry.Grade?.ToString() ?? "нет")}"
                    );
                }
            }

            await _context.SaveChangesAsync();

            // 🔥 1 лог на урок (основной)
            await _actionService.LogAsync(
                "Заполнение журнала",
                "Журнал",
                exists.Id,
                $"Класс: {exists.Class.NumClass}-{exists.Class.LetterClass}, " +
                $"\nПредмет: {exists.Subject.Name}. " +
                "\n" + string.Join(". ", changes) + $" \nОценки выставлены: {exists.Entries
                .Where(s => s.Grade != null).Count()}/{exists.Entries.Count()}" +
                $"\nОтсутствовало: {exists.Entries.Where(e => !e.IsPresent).Count()}"
            );
          
            // 🔥 отдельный лог только если были изменения по ученикам
            if (entryChanges.Any())
            {
                await _actionService.LogAsync(
                    "Изменение записей в журнале",
                    "Журнал",
                    exists.Id,
                    string.Join("; ", entryChanges)
                );
            }

            return OperationResult<string>.Ok(journal.Id, "Журнал успешно сохранен");
        }
        public async Task<OperationResult<Journal>> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return OperationResult<Journal>.Fail("Айди пуст редактирование журнала невозможно");
            var journal = await _context.Journal
                    .Include(c => c.Class)
                    .Include(t => t.Teacher)
                    .ThenInclude(t => t.User)
                    .Include(s => s.Subject)
                    .Include(j => j.Entries)
                    .ThenInclude(j => j.Student)
                    .ThenInclude(j => j.User)
                    .Include(j => j.AcademicYear)
                    .Include(j => j.Term)
                    .FirstOrDefaultAsync(j => j.Id == id);
            if (journal == null)
                return OperationResult<Journal>.Fail("Журнал не найден для редактирования");
            DateTime now = DateTime.Now;
            if (journal.Date != default)
            {
                journal.IsLocked = (DateTime.Now - journal.Date).TotalDays <= 7;
            }
            else
            {
                journal.IsLocked = true; // ещё нет записей, редактировать можно
            }
            return OperationResult<Journal>.Ok(journal, null);
        }
        private async Task<AcademicYear?> GetAcademicYear(DateTime dayToday) => await _context.AcademicYear
                .Where(d => d.StartDateYear <= dayToday && dayToday <= d.EndDateYear)
                .FirstOrDefaultAsync();
        private async Task<Term?> GetTerm(DateTime dayToday, string currentId) => await _context.Term
           .Where(d => d.AcademicYearId == currentId && d.DateStartTerm <= dayToday && dayToday <= d.DateEndTerm)
           .FirstOrDefaultAsync();
    }
}
