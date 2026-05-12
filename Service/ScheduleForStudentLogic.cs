using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Service
{
    public class ScheduleForStudentLogic
    {
        private readonly AppDbContext _context;
        public ScheduleForStudentLogic(AppDbContext context)
        {
            _context = context;
        }
        public class OperationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public static OperationResult Ok() => new OperationResult { Success = true, };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message};
        }
        public async Task<object> ScheduleLesson(string dayOfWeek, string studentId)
        {
            var classId = await _context.Students
               .Include(u => u.User)
               .Include(u => u.Class)
               .Where(s => s.Id == studentId)
               .Select(s => s.ClassId)
               .FirstOrDefaultAsync();
            var scheduleList = await _context.Schedules
                .Include(s => s.LessonSlot)
                .Include(s => s.Assigment)
                .ThenInclude(s => s.Class)
                .Include(s => s.Assigment)
                .ThenInclude(s => s.Teacher)
                .ThenInclude(s => s.User)
                .Where(s => s.DayOfWeek.ToLower().Trim() == dayOfWeek.ToLower().Trim() && s.Assigment.ClassId == classId)
                .Select(s => new
                {
                    lessonNumber = s.LessonSlot.LessonNumber,
                    timeStart = s.LessonSlot.StartTime,
                    timeEnd = s.LessonSlot.EndTime,
                    subject = s.Assigment.Subject.Name,
                    teacher = s.Assigment.Teacher.User.FullName,
                    room = s.Room
                })
                .ToListAsync();
            return scheduleList;
        }
    }
}
