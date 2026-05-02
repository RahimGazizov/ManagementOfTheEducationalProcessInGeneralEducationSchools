using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class AcademicYearLogic
    {
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionLogService;
        public AcademicYearLogic(AppDbContext context, ActionLogService actionLogService)
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
        public async Task<OperationResult> AddAcademicYear(AcademicYear fm)
        {
            if (fm == null)
                return OperationResult.Fail("Данные не были заполнены, добавление невозможно");
            var exists = await _context.AcademicYear.AnyAsync(a => a.Name == fm.Name);
            if (exists)
                return OperationResult.Fail($"Название учебного года {fm.Name} уже существует");
            exists = await _context.AcademicYear
               .AnyAsync(s => s.StartDateYear == fm.StartDateYear && s.EndDateYear == fm.EndDateYear);
            if (exists)
                return OperationResult.Fail("Такая дата начало и конца уже существует");
            if (fm.StartDateYear > fm.EndDateYear)
                return OperationResult.Fail($"Дата начало должна быть меньше даты конца");

            string[] date = fm.Name.Split("-");
            if (date[0].Length < 4 || date[1].Length < 4)
                return OperationResult.Fail($"Введите дату в 4-х значном формате");

            if (Convert.ToDouble(date[0]) > Convert.ToDouble(date[1]))
                return OperationResult.Fail($"Название даты начало {date[0]} не должно быть больше чем название даты конца {date[1]}");

            _context.AcademicYear.Add(fm);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
                "Добавление учебного года",
                "Учебный год",
                fm.Id,
                $"Добавлен учебный год: Название: {fm.Name}. Дата начало: {fm.StartDateYear.ToString("dd-MM-yyyy")}." +
                $" Дата конца: {fm.EndDateYear.ToString("dd-MM-yyyy")}"
                );
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EditAcademicYear(AcademicYear fm)
        {
            if (fm == null)
                return OperationResult.Fail("Данные не были заполнены, редактирование невозможно");

            var exists = await _context.AcademicYear
                .AnyAsync(a => a.Name == fm.Name && a.Id != fm.Id);

            if (exists)
                return OperationResult.Fail($"Название учебного года {fm.Name} уже существует");
            exists = await _context.AcademicYear
                .AnyAsync(s => s.StartDateYear == fm.StartDateYear && s.EndDateYear == fm.EndDateYear && s.Id != fm.Id);
            if (exists)
                return OperationResult.Fail("Такая дата начало и конца уже существует");
            if (fm.StartDateYear > fm.EndDateYear)
                return OperationResult.Fail("Дата начала должна быть меньше даты конца");

            // Проверка формата "2023-2024"
            var dateParts = fm.Name.Split('-');
            if (dateParts.Length != 2)
                return OperationResult.Fail("Неверный формат названия (ожидается 2023-2024)");

            if (!int.TryParse(dateParts[0], out var startYear) ||
                !int.TryParse(dateParts[1], out var endYear))
                return OperationResult.Fail("Год должен быть числом");

            if (dateParts[0].Length != 4 || dateParts[1].Length != 4)
                return OperationResult.Fail("Введите год в 4-значном формате");

            if (startYear > endYear)
                return OperationResult.Fail($"Год начала {startYear} не должен быть больше года конца {endYear}");

            // Получаем сущность из БД (отслеживаемую)
            var entity = await _context.AcademicYear
                .FirstOrDefaultAsync(s => s.Id == fm.Id);

            if (entity == null)
                return OperationResult.Fail("Запись учебного года не найдена");

            // Сохраняем старые значения (ВАЖНО для лога)
            var oldName = entity.Name;
            var oldStart = entity.StartDateYear;
            var oldEnd = entity.EndDateYear;

            // Обновляем вручную
            entity.Name = fm.Name;
            entity.StartDateYear = fm.StartDateYear;
            entity.EndDateYear = fm.EndDateYear;

            await _context.SaveChangesAsync();

            // Лог
            await _actionLogService.LogAsync(
                "Редактирование учебного года",
                "Учебный год",
                entity.Id,
                $"Название: {oldName} → {entity.Name}. " +
                $"Дата начала: {oldStart:dd-MM-yyyy} → {entity.StartDateYear:dd-MM-yyyy}. " +
                $"Дата конца: {oldEnd:dd-MM-yyyy} → {entity.EndDateYear:dd-MM-yyyy}"
            );

            return OperationResult.Ok();
        }
        public async Task<OperationResult> Delete(string id)
        {
            if (id == null)
                return OperationResult.Fail("Айди пришел пустым, удаление невозможно");
            var exists = await _context.AcademicYear.FindAsync(id);
            if (exists == null)
                return OperationResult.Fail("Обьект не найден удаление невозможно");
            _context.AcademicYear.Remove(exists);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
               "Удаление учебного года",
               "Учебный год",
               id,
               $"Удален учебный год: Название: {exists.Name}. Дата начало: {exists.StartDateYear.ToString("dd-MM-yyyy")}." +
               $" Дата конца: {exists.EndDateYear.ToString("dd-MM-yyyy")}"
               );
            return OperationResult.Ok();
        }
    }
}
