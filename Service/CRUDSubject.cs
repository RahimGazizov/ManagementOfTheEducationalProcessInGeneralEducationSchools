using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.EntityFrameworkCore;
using static InformationSystemOfASchoolIducationalPortal.Service.CRUDClass;

namespace InformationSystemOfASchoolIducationalPortal.Service
{
    public class CRUDSubject
    {
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionLogService;
        public CRUDSubject(AppDbContext context, ActionLogService actionLogService)
        {
            _context = context;
            _actionLogService = actionLogService;
        }
        public class OperationResultSubject
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public static OperationResultSubject Ok() => new OperationResultSubject { Success = true };
            public static OperationResultSubject Fail(string message) => new OperationResultSubject { Success = false, Message = message };
        }
        public async Task<OperationResultSubject> Add(string subjectName)
        {
            bool exists = await _context.Subjects
     .AnyAsync(n => EF.Functions.Like(n.Name, subjectName));
            if (exists)
                return OperationResultSubject.Fail("Такой предмет уже есть в списке");
            var sub = new Subjects { Name = subjectName };
            _context.Subjects.Add(sub);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
      action: "Создание предмета",
         entityName: "Предметы",
         entityId: sub.Id,
         details: $"Создан предмет: {sub.Name}.");
            return OperationResultSubject.Ok();
        }
        public async Task<OperationResultSubject> Delete(string id)
        {
            var sub = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            if (sub == null)
                return OperationResultSubject.Fail("Предмет не найден");
            var oldSub = sub.Name;
            var subId = sub.Id;
            _context.Subjects.Remove(sub);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
              action: "Удаление предмета",
              entityName: "Предметы",
              entityId: subId,
              details: $"Удален предмет: {sub.Name}.");
            return OperationResultSubject.Ok();
        }
        public async Task<OperationResultSubject> Edit(string id, string subjectName)
        {
            var sub = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            if (sub == null)
                return OperationResultSubject.Fail("Предмет не найден. Редоктирование отменено");
            bool exists = await _context.Subjects.AnyAsync(n => EF.Functions.Like(n.Name, subjectName));
            if (exists) return OperationResultSubject.Fail("Такой предмет уже есть в списке");
            var oldSub = sub.Name;
            sub.Name = subjectName;
            _context.Subjects.Update(sub);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
            action: "Редактирование предмета",
            entityName: "Предметы",
            entityId: sub.Id,
            details: $"Изменен предмет: {oldSub} → {subjectName}.");
            return OperationResultSubject.Ok();
        }
    }
}
