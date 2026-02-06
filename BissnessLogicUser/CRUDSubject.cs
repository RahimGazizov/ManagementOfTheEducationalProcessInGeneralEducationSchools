using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class CRUDSubject
    {
        private readonly AppDbContext _context;
        public CRUDSubject(AppDbContext context)
        {
            _context = context;
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
            return OperationResultSubject.Ok();
        }
        public async Task<OperationResultSubject> Delete(string id)
        {
            var sub = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            if (sub == null)
                return OperationResultSubject.Fail("Предмет не найден");

            _context.Subjects.Remove(sub);
            await _context.SaveChangesAsync();
            return OperationResultSubject.Ok();
        }
        public async Task<OperationResultSubject> Edit(string id, string subjectName)
        {
            var sub = await _context.Subjects.FirstOrDefaultAsync(x => x.Id == id);
            if (sub == null)
                return OperationResultSubject.Fail("Предмет не найден. Редоктирование отменено");
            bool exists = await _context.Subjects.AnyAsync(n => EF.Functions.Like(n.Name, subjectName));
            if (exists) return OperationResultSubject.Fail("Такой предмет уже есть в списке");
            sub.Name = subjectName;
            _context.Subjects.Update(sub);
            await _context.SaveChangesAsync();
            return OperationResultSubject.Ok();
        }
    }
}
