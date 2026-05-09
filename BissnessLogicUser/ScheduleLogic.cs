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
        private readonly ActionLogService _actionLogService;
        public ScheduleLogic(AppDbContext context, ActionLogService actionLogService)
        {
            _context = context;
            _actionLogService = actionLogService;
        }
        public class OperationResult
        {
            public string Message { get; set; }
            public bool Success { get; set; }
            public static OperationResult Ok() { return new OperationResult { Success = true }; }
            public static OperationResult Fail(string message) { return new OperationResult { Success = false, Message = message }; }
        }
        private class ScheduleDto
        {
            public string TeacherName { get; set; }
            public int ClassNum { get; set; }
            public string ClassLett { get; set; }
            public string SubName { get; set; }
            public string Room { get; set; }
            public string StartTime { get; set; }
            public string EndTime { get; set; }
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
            var exists = await _context.Schedules.AnyAsync(s => s.TeacherAssigmentId == fm.TeacherAssigmentId &&
            s.LessonSlotId == fm.LessonSlotId && s.DayOfWeek == fm.DayOfWeek && s.Room == fm.Room);
            if (exists)
                return OperationResult.Fail("Такая запись уже существует!");
            var teacherId = await _context.TeacherAssigments
               .Where(t => t.Id == fm.TeacherAssigmentId)
               .Select(t => t.TeacherId)
               .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(s => s.Assigment.TeacherId == teacherId &&
            s.DayOfWeek == fm.DayOfWeek && s.LessonSlotId == fm.LessonSlotId);
            if (exists)
                return OperationResult.Fail("У учителя в это время стоит урок!");
            var classId = await _context.TeacherAssigments
               .Where(t => t.Id == fm.TeacherAssigmentId)
               .Select(t => t.ClassId)
               .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(s => s.Assigment.ClassId == classId &&
            s.DayOfWeek == fm.DayOfWeek && s.LessonSlotId == fm.LessonSlotId);
            if (exists)
                return OperationResult.Fail("У класса в это время стоит урок!");
            var subName = await _context.TeacherAssigments
                .Where(t => t.Id == fm.TeacherAssigmentId)
                .Select(t => t.Subject.Name)
                .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(t => t.DayOfWeek == fm.DayOfWeek && t.LessonSlotId == fm.LessonSlotId &&
                t.Room == fm.Room);
            if (exists)
                return OperationResult.Fail("Кабинет занят другим классом");
            var count = await _context.Schedules.
                Where(t => t.Assigment.ClassId == classId && t.Assigment.Subject.Name == subName)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Subject)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Class)
                .CountAsync();
            if (count == 2)
                return OperationResult.Fail("Нельзя добавлять один предмет для класса больше 2 в неделю");
            var acedemic = await _context.AcademicYear.Where(s => DateTime.Now >= s.StartDateYear && DateTime.Now <= s.EndDateYear)
                .FirstOrDefaultAsync();
            fm.AcademicYearId = acedemic.Id;
            _context.Schedules.Add(fm);
            await _context.SaveChangesAsync();
            var assigment = await _context.Schedules
                .Where(s => s.Id == fm.Id)
                .Select(s => new
                {
                    teacheName = s.Assigment.Teacher.User.FullName,
                    classNum = s.Assigment.Class.NumClass,
                    classLett = s.Assigment.Class.LetterClass,
                    subName = s.Assigment.Subject.Name,
                    room = s.Room,
                    startTime = s.LessonSlot.StartTime.ToString(@"hh\:mm"),
                    endTime = s.LessonSlot.EndTime.ToString(@"hh\:mm"),
                }).FirstOrDefaultAsync();
            if (assigment == null)
                return OperationResult.Fail("Данные расписания для лога не найдена");
            await _actionLogService.LogAsync(
                "Добавление расписание",
                "Расписание",
                fm.Id,
                $"Добавлено расписание: Учитель: {assigment.teacheName}. Класс: {assigment.classNum}-{assigment.classLett}. " +
                $"Предмет: {assigment.subName}. Время урока: {assigment.startTime}-{assigment.endTime}. Кабинет: {assigment.room}"
                );
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EditScheduleClass(ScheduleLesson fm)
        {
            var exists = await _context.Schedules.AnyAsync(s => s.TeacherAssigmentId == fm.TeacherAssigmentId &&
            s.LessonSlotId == fm.LessonSlotId && s.DayOfWeek == fm.DayOfWeek && s.Room == fm.Room && s.Id != fm.Id);
            if (exists)
                return OperationResult.Fail("Такая запись уже существует!");
            var teacherId = await _context.TeacherAssigments
               .Where(t => t.Id == fm.TeacherAssigmentId)
               .Select(t => t.TeacherId)
               .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(s => s.Assigment.TeacherId == teacherId &&
            s.DayOfWeek == fm.DayOfWeek && s.LessonSlotId == fm.LessonSlotId && s.Id != fm.Id);
            if (exists)
                return OperationResult.Fail("У учителя в это время стоит урок!");
            var classId = await _context.TeacherAssigments
               .Where(t => t.Id == fm.TeacherAssigmentId)
               .Select(t => t.ClassId)
               .FirstOrDefaultAsync();
            exists = await _context.Schedules
                .AnyAsync(s => s.Assigment.ClassId == classId &&
            s.DayOfWeek == fm.DayOfWeek && s.LessonSlotId == fm.LessonSlotId && s.Id != fm.Id);
            if (exists)
                return OperationResult.Fail("У класса в это время стоит урок!");
            var subName = await _context.TeacherAssigments
                .Where(t => t.Id == fm.TeacherAssigmentId)
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
            var oldSchedule = await GetSchedule(fm.Id);
            if (oldSchedule == null)
                return OperationResult.Fail("Старые записи не найдены для лога");
            schedule.DayOfWeek = fm.DayOfWeek;
            schedule.LessonSlotId = fm.LessonSlotId;
            schedule.TeacherAssigmentId = fm.TeacherAssigmentId;
            schedule.Room = fm.Room;
            await _context.SaveChangesAsync();
            var newSchedule = await GetSchedule(schedule.Id);
            if (newSchedule == null)
                return OperationResult.Fail("Данные расписания для лога не найдена");
            await _actionLogService.LogAsync(
                "Редактирование расписание",
                "Расписание",
                fm.Id,
                $"Изменено расписание: Учитель:{oldSchedule.TeacherName} → {newSchedule.TeacherName}. Класс: {oldSchedule.ClassNum}-{oldSchedule.ClassLett} → {newSchedule.ClassNum}-{newSchedule.ClassLett}. " +
                $"Предмет: {newSchedule.SubName} → {newSchedule.SubName}. Время урока: {oldSchedule.StartTime}-{oldSchedule.EndTime} → {newSchedule.StartTime}-{newSchedule.EndTime}. Кабинет: {oldSchedule.Room} → {newSchedule.Room}"
                );
            return OperationResult.Ok();
        }
        public async Task<OperationResult> Delete(string id)
        {
            if (id == null)
                return OperationResult.Fail("Айди обекта пуст");
            var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.Id == id);
            if (schedule == null)
                return OperationResult.Fail("Раписание не найдено по выбраному объекту");
            var getSchedule = await GetSchedule(id);
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            await _actionLogService.LogAsync(
                "Удаление расписание",
                "Расписание",
                id,
                $"Удалено расписание: Учитель: {getSchedule.TeacherName}. Предмет: {getSchedule.SubName}. Класс: {getSchedule.ClassNum}-{getSchedule.ClassLett}" +
                $" Время урока: {getSchedule.StartTime}-{getSchedule.EndTime}. Кабинет: {getSchedule.Room}"
                );
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
                .OrderBy(t => t.LessonNumber)
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
                    .Include(s => s.AcademicYear)
                    .ToListAsync();
            return sch;
        }
        private async Task<ScheduleDto> GetSchedule(string id)
        {
            var sch = await _context.Schedules
                .Where(s => s.Id == id)
                .Select(s => new ScheduleDto
                {
                    TeacherName = s.Assigment.Teacher.User.FullName,
                    ClassNum = s.Assigment.Class.NumClass,
                    ClassLett = s.Assigment.Class.LetterClass,
                    SubName = s.Assigment.Subject.Name,
                    Room = s.Room,
                    StartTime = s.LessonSlot.StartTime.ToString(@"hh\:mm"),
                    EndTime = s.LessonSlot.EndTime.ToString(@"hh\:mm"),
                }).FirstOrDefaultAsync();
            return sch;
        }
    }
}
