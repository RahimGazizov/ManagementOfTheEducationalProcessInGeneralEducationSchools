using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Reflection.Emit;
using static InformationSystemOfASchoolIducationalPortal.Service.CRUDClass;
namespace InformationSystemOfASchoolIducationalPortal.Service
{
    public class TeachingAssigmentLogic
    {
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionLogService;
        public TeachingAssigmentLogic(AppDbContext context, ActionLogService actionLogService)
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
        private class OldAssigment
        {
            public string TeacherName { get; set; }
            public string SubjectName { get; set; }
            public int ClassNum { get; set; }
            public string ClassLetter { get; set; }
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
            if (assigment == null)
                return OperationResult.Fail("Данные назначения не переданы");

            var exists = await _context.TeacherAssigments
                .AnyAsync(a =>
                    a.TeacherId == assigment.TeacherId &&
                    a.SubjectId == assigment.SubjectId &&
                    a.ClassId == assigment.ClassId);

            if (exists)
                return OperationResult.Fail("Такая сущность уже есть");

            var currentAcademic = await _context.AcademicYear
                .FirstOrDefaultAsync(s =>
                    s.StartDateYear <= DateTime.Now &&
                    DateTime.Now <= s.EndDateYear);

            if (currentAcademic == null)
                return OperationResult.Fail("Добавьте учебный год");

            var newAssigment = new TeacherAssigment
            {
                TeacherId = assigment.TeacherId,
                SubjectId = assigment.SubjectId,
                ClassId = assigment.ClassId,
                AcademicId = currentAcademic.Id,
            };

            _context.TeacherAssigments.Add(newAssigment);
            await _context.SaveChangesAsync();

            var logData = await _context.TeacherAssigments
                .Where(s => s.Id == newAssigment.Id)
                .Select(s => new
                {
                    TeacherName = s.Teacher.User.FullName,
                    SubjectName = s.Subject.Name,
                    ClassNum = s.Class.NumClass,
                    ClassLetter = s.Class.LetterClass
                })
                .FirstOrDefaultAsync();

            if (logData == null)
                return OperationResult.Fail("Назначение создано, но данные для лога не найдены");

            await _actionLogService.LogAsync(
                action: "Добавление назначения учителя",
                entityName: "TeachingAssignment",
                entityId: newAssigment.Id.ToString(),
                details: $"Добавлено назначение учителя. Учитель: {logData.TeacherName}. " +
                         $"Предмет: {logData.SubjectName}. " +
                         $"Класс: {logData.ClassNum}-{logData.ClassLetter}."
            );

            return OperationResult.Ok();
        }
        public async Task<OperationResult> Edit(TeacherAssigment assigment)
        {
            if (assigment == null)
                return OperationResult.Fail("Данные назначения не переданы");
    
            var findAssigment = await _context.TeacherAssigments
       .Include(t => t.Teacher)
           .ThenInclude(u => u.User)
       .Include(t => t.Subject)
       .Include(t => t.Class)
       .FirstOrDefaultAsync(t => t.Id == assigment.Id); // t — элемент из БД
            if (findAssigment == null)
                return OperationResult.Fail("Запись не найдена");

            if (findAssigment.Subject == null)
                return OperationResult.Fail($"Предмет не найден. SubjectId: {findAssigment.SubjectId}");
            var exists = await _context.TeacherAssigments
             .AnyAsync(a => a.TeacherId == assigment.TeacherId &&
             a.SubjectId == assigment.SubjectId && a.ClassId == assigment.ClassId && a.Id != assigment.Id);
            if (exists)
                return OperationResult.Fail("Такая сущность уже есть");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"SubjectName - {findAssigment.Subject.Name}");
            Console.ForegroundColor = ConsoleColor.White;
            var oldAssigment = new OldAssigment
            {
                TeacherName = findAssigment.Teacher.User.FullName,
                SubjectName = findAssigment.Subject.Name,
                ClassNum = findAssigment.Class.NumClass,
                ClassLetter = findAssigment.Class.LetterClass
            };
            findAssigment.TeacherId = assigment.TeacherId;
            findAssigment.SubjectId = assigment.SubjectId;
            findAssigment.ClassId = assigment.ClassId;
            _context.TeacherAssigments.Update(findAssigment);
            await _context.SaveChangesAsync();
            var subName = await _context.TeacherAssigments.Include(s => s.Subject)
                .Where(s => s.Id == findAssigment.Id).Select(s => s.Subject.Name)
                .FirstOrDefaultAsync();
            await _actionLogService.LogAsync(
 action: "Назначение учителей редоктирование",
 entityName: "TeachingAssignment",
 entityId: findAssigment.Id,
 details: $"Изменено назначение учителя. " +
              $"Учитель: {oldAssigment.TeacherName} → {findAssigment.Teacher.User.FullName}. " +
              $"\nПредмет: {oldAssigment.SubjectName} → {subName}. " +
              $"\nКласс: {oldAssigment.ClassNum}-{oldAssigment.ClassLetter} → {findAssigment.Class.NumClass}-{findAssigment.Class.LetterClass}.");
            return OperationResult.Ok();
        }
        public async Task<OperationResult> Delete(string id)
        {
            var assigment = await _context.TeacherAssigments
                .Include(a => a.Teacher)
                    .ThenInclude(t => t.User)
                .Include(a => a.Subject)
                .Include(a => a.Class)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assigment == null)
                return OperationResult.Fail("Данные не найдены");

            var assigmentId = assigment.Id.ToString();

            var teacherName = assigment.Teacher?.User?.FullName ?? "Не указан";
            var subjectName = assigment.Subject?.Name ?? "Не указан";
            var className = assigment.Class != null
                ? $"{assigment.Class.NumClass}-{assigment.Class.LetterClass}"
                : "Не указан";

            _context.TeacherAssigments.Remove(assigment);
            await _context.SaveChangesAsync();

            await _actionLogService.LogAsync(
                action: "Удаление назначения учителя",
                entityName: "TeachingAssignment",
                entityId: assigmentId,
                details: $"Удалено назначение учителя. Учитель: {teacherName}. Предмет: {subjectName}. Класс: {className}."
            );

            return OperationResult.Ok();
        }
    }
}