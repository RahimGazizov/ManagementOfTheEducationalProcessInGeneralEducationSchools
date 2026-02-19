using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class TeachingAssigmentLogic
    {
        private readonly AppDbContext _context;
        public TeachingAssigmentLogic(AppDbContext context)
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
        public async Task<List<SelectListItem>> GetListTeachers()
        {
            var list = await _context.Teachers.Select(t => new SelectListItem
            {
                Value = t.Id,
                Text = t.User.FullName,
            }).ToListAsync();
            return list;
        }
        public async Task<List<SelectListItem>> GetListSubjects()
        {
            var list = await _context.Subjects.Select(s => new SelectListItem
            {
                Value = s.Id,
                Text = s.Name,
            }).ToListAsync();
            return list;
        }
        public async Task<OperationResult> Add(TeacherAssigment assigment)
        {
            var exists = await _context.TeacherAssigments
               .AnyAsync(a => a.TeacherId == assigment.TeacherId &&
               a.SubjectId == assigment.SubjectId && a.ClassId == assigment.ClassId);
            if (exists)
                return OperationResult.Fail("Такая сущность уже есть");
            var newAssigment = new TeacherAssigment
            {
                TeacherId = assigment.TeacherId,
                SubjectId = assigment.SubjectId,
                ClassId = assigment.ClassId,
            };
            _context.TeacherAssigments.Add(newAssigment);
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }
        public async Task<OperationResult> Edit(TeacherAssigment assigment)
        {
            var findAssigment = await _context.TeacherAssigments
       .Include(t => t.Teacher)
           .ThenInclude(u => u.User)
       .Include(t => t.Subject)
       .Include(t => t.Class)
       .FirstOrDefaultAsync(t => t.Id == assigment.Id); // t — элемент из БД
            if (findAssigment == null)
                return OperationResult.Fail("Запись не найдена");

            var exists = await _context.TeacherAssigments
             .AnyAsync(a => a.TeacherId == assigment.TeacherId &&
             a.SubjectId == assigment.SubjectId && a.ClassId == assigment.ClassId && a.Id != assigment.Id);
            if (exists)
                return OperationResult.Fail("Такая сущность уже есть");

            findAssigment.TeacherId = assigment.TeacherId;
            findAssigment.SubjectId = assigment.SubjectId;
            findAssigment.ClassId = assigment.ClassId;
            _context.TeacherAssigments.Update(findAssigment);
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }
    }
}