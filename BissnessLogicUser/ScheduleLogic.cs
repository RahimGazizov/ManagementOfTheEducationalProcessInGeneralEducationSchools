using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class ScheduleLogic
    {
        private readonly AppDbContext _context;
        public ScheduleLogic(AppDbContext context)
        {
            _context = context;
        }
        public class OperationResult
        {
            public string Message { get; set; }
            public bool Success { get; set; }
            public static OperationResult Ok() { return new OperationResult { Success = true }; }
            public static OperationResult Fail(string message) { return new OperationResult { Success = false, Message = message }; }
        }
        public async Task<ScheduleLessonViewModal> ForIndex()
        {
            var sch = await ScheduleLessons();
            var schedule = new ScheduleLessonViewModal
            {
                ScheduleLessons = sch,
            };
            return schedule;
        }
        public async Task<OperationResult> AddScheduleClass(ScheduleLesson fm)
        {
            var exists = await _context.Schedules.AnyAsync(s => s.TeachinsAssignmentId == fm.TeachinsAssignmentId &&
            s.LessonSlotId == fm.LessonSlotId && s.DayOfWeek == fm.DayOfWeek && s.Room == fm.Room);
            if (exists)
                return OperationResult.Fail("Такая запись уже существует!");
            var teacherId = await _context.TeacherAssigments
               .Where(t => t.Id == fm.TeachinsAssignmentId)
               .Select(t => t.TeacherId)
               .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(s => s.Assigment.TeacherId == teacherId &&
            s.DayOfWeek == fm.DayOfWeek && s.LessonSlotId == fm.LessonSlotId);
            if (exists)
                return OperationResult.Fail("У учителя в это время стоит урок!");
            var classId = await _context.TeacherAssigments
               .Where(t => t.Id == fm.TeachinsAssignmentId)
               .Select(t => t.ClassId)
               .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(s => s.Assigment.ClassId == classId &&
            s.DayOfWeek == fm.DayOfWeek && s.LessonSlotId == fm.LessonSlotId);
            if (exists)
                return OperationResult.Fail("У класса в это время стоит урок!");
            var subName = await _context.TeacherAssigments
                .Where(t => t.Id == fm.TeachinsAssignmentId)
                .Select(t => t.Subject.Name)
                .FirstOrDefaultAsync();
            var count = await _context.Schedules.
                Where(t => t.Assigment.ClassId == classId && t.Assigment.Subject.Name == subName)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Subject)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Class)
                .CountAsync();
            if (count == 2)
                return OperationResult.Fail("Нельзя добавлять один предмет для класса больше 2 в неделю");
            _context.Schedules.Add(fm);
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EditScheduleClass(ScheduleLesson fm)
        {
            var exists = await _context.Schedules.AnyAsync(s => s.TeachinsAssignmentId == fm.TeachinsAssignmentId &&
            s.LessonSlotId == fm.LessonSlotId && s.DayOfWeek == fm.DayOfWeek && s.Room == fm.Room && s.Id != fm.Id);
            if (exists)
                return OperationResult.Fail("Такая запись уже существует!");
            var teacherId = await _context.TeacherAssigments
               .Where(t => t.Id == fm.TeachinsAssignmentId)
               .Select(t => t.TeacherId)
               .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(s => s.Assigment.TeacherId == teacherId &&
            s.DayOfWeek == fm.DayOfWeek && s.LessonSlotId == fm.LessonSlotId && s.Id != fm.Id);
            if (exists)
                return OperationResult.Fail("У учителя в это время стоит урок!");
            var classId = await _context.TeacherAssigments
               .Where(t => t.Id == fm.TeachinsAssignmentId)
               .Select(t => t.ClassId)
               .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(s => s.Assigment.ClassId == classId &&
            s.DayOfWeek == fm.DayOfWeek && s.LessonSlotId == fm.LessonSlotId && s.Id != fm.Id);
            if (exists)
                return OperationResult.Fail("У класса в это время стоит урок!");
            var subName = await _context.TeacherAssigments
                .Where(t => t.Id == fm.TeachinsAssignmentId)
                .Select(t => t.Subject.Name)
                .FirstOrDefaultAsync();
            var count = await _context.Schedules.
                Where(t => t.Assigment.ClassId == classId && t.Assigment.Subject.Name == subName && t.Id != fm.Id)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Subject)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Class)
                .CountAsync();
            if (count >= 2)
                return OperationResult.Fail("Нельзя добавлять один предмет для класса больше 2 в неделю");
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == fm.Id);
            schedule.DayOfWeek = fm.DayOfWeek;
            schedule.LessonSlotId = fm.LessonSlotId;
            schedule.TeachinsAssignmentId = fm.TeachinsAssignmentId;
            schedule.Room = fm.Room;
            await _context.SaveChangesAsync();
            return OperationResult.Ok();
        }
        public List<SelectListItem> GetListDaysOfWeek()
        {
            var culture = new CultureInfo("ru-RU");
            var listDays = culture.DateTimeFormat.DayNames.Select(d => new SelectListItem
            {
                Text = d,
                Value = d
            }).ToList();
            return listDays;
        }
        public async Task<List<SelectListItem>> ListAssigment()
        {
            var assigmentList = await _context.TeacherAssigments
                .Include(t => t.Teacher)
                .ThenInclude(t => t.User)
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .Select(l => new SelectListItem
                {
                    Value = l.Id,
                    Text = $"{l.Subject.Name} - {l.Class.NumClass + l.Class.LetterClass} ({l.Teacher.User.FullName})"
                })
                .ToListAsync();
            return assigmentList;
        }
        public async Task<List<SelectListItem>> LessonSlotList()
        {
            var lessonSlot = await _context.LessonSlots
                .Select(l => new SelectListItem
                {
                    Value = l.Id,
                    Text = $"{l.LessonNumber} - {l.StartTime.ToString(@"hh\:mm")}-{l.EndTime.ToString(@"hh\:mm")}"
                })
                .ToListAsync();
            return lessonSlot;
        }
        public async Task<List<ScheduleLesson>> ScheduleLessons()
        {
            var sch = await _context.Schedules
                    .Include(t => t.Assigment)
                    .ThenInclude(t => t.Subject)
                    .Include(t => t.Assigment)
                    .ThenInclude(t => t.Teacher)
                    .ThenInclude(t => t.User)
                    .Include(t => t.Assigment)
                    .ThenInclude(t => t.Class)
                    .Include(t => t.LessonSlot)
                    .ToListAsync();
            return sch;
        }
    }
}
