using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class CRUDClass
    {
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionLogService;
        public CRUDClass(AppDbContext context, ActionLogService actionLogService)
        {
            _context = context;
            _actionLogService = actionLogService;
        }
        public class OperationResultClass
        {
            public bool Suceeced { get; set; }
            public string Message { get; set; }
            public static OperationResultClass OK() => new OperationResultClass { Suceeced = true };
            public static OperationResultClass Fail(string message) => new OperationResultClass { Suceeced = false, Message = message };

        }
        public async Task<OperationResultClass> AddClass(int numClass, string letterClass)
        {
            if (_context.Classes.FirstOrDefault(c => c.LetterClass == letterClass && c.NumClass == numClass) != null)
                return OperationResultClass.Fail("Такой класс уже существует");
            var currentAcademic = await _context.AcademicYear
                .Where(s => s.StartDateYear <= DateTime.Now && DateTime.Now <= s.EndDateYear).FirstOrDefaultAsync();
            if (currentAcademic == null)
                return OperationResultClass.Fail("Добавьте учебный год");
            var classes = new Class
            {
                NumClass = numClass,
                LetterClass = letterClass,
                AcademicYearId = currentAcademic.Id
            };
            await _context.Classes.AddAsync(classes);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
         action: "Создание класса",
         entityName: "Class",
         entityId: classes.Id,
         details: $"Создан класс: {classes.NumClass}-{classes.LetterClass}.");
            return OperationResultClass.OK();
        }
        public async Task<OperationResultClass> Delete(string id)
        {
            var _class = await _context.Classes.FirstOrDefaultAsync(i => i.Id == id);
            if (_class == null)
                return OperationResultClass.Fail("Класс не найден");
            var oldClassName = $"{_class.NumClass}-{_class.LetterClass}";
            var classID = _class.Id;
            _context.Classes.Remove(_class);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
      action: "Удаление класса",
         entityName: "Class",
         entityId: classID,
         details: $"Удален класс: {oldClassName}.");
            return OperationResultClass.OK();
        }
        public async Task<OperationResultClass> Edit(string id, int numClass, string letterClass)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls == null)
                return OperationResultClass.Fail("Класс не найден");
            if (_context.Classes.FirstOrDefault(c => c.LetterClass == letterClass && c.NumClass == numClass) != null)
                return OperationResultClass.Fail("Такой класс уже существует");
            var oldClassName = $"{cls.NumClass}-{cls.LetterClass}";
            cls.NumClass = numClass;
            cls.LetterClass = letterClass;
            _context.Classes.Update(cls);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
      action: "Редактирование класса",
         entityName: "Class",
         entityId: cls.Id,
         details: $"Изменен класс: {oldClassName} → {cls.NumClass}-{cls.LetterClass}.");
            return OperationResultClass.OK();
        }
    }
}