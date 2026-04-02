using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace InformationSystemOfASchoolIducationalPortal.BissnessLogicUser
{
    public class StudentPerAccLogic
    {
        private readonly AppDbContext _context;
        public StudentPerAccLogic(AppDbContext context)
        {
            _context = context;
        }
        public class OperationResult
        {
            public string Message { get; set; }
            public bool Success { get; set; }
            public static OperationResult Ok() => new OperationResult { Success = true };
            public static OperationResult Fail(string message) => new OperationResult { Success = false, Message = message };
        }
        public async Task<AcademicYear> GetCurrentAcademicYear()
        {
            return await _context.AcademicYear
               .Where(d => d.StartDateYear <= DateTime.Now && d.EndDateYear >= DateTime.Now)
               .FirstOrDefaultAsync() ?? new();
        }
        public async Task<Term> GetCurrentTerm()
        {
            return await _context.Term
               .Where(d => d.DateStartTerm <= DateTime.Now && d.DateEndTerm >= DateTime.Now)
               .FirstOrDefaultAsync() ?? new();
        }
        public async Task<List<Subjects>> ListSubjects(string classId)
        {
            return await _context.TeacherAssigments
                    .Where(t => t.ClassId == classId)
                    .Include(s => s.Subject)
                    .Select(s => s.Subject)
                    .ToListAsync();
        }
        public async Task<double> AverageGrade(Students student)
        {
            var avr = await _context.JournalEntry
                   .Where(t => t.StudentId == student.Id && t.Grade != null)
                   .AverageAsync(e => e.Grade);
            return avr != null ? Convert.ToDouble(avr) : 0;
        }
        public async Task<List<ScheduleLesson>> ScheduleLessons(Students student)
        {
            var dayToday = DateTime.Today.ToString("dddd", new CultureInfo("ru-RU")).ToLower().Trim();
            var schedule = await _context.Schedules
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Subject)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Class)
                .Include(t => t.LessonSlot)
                .Include(t => t.Assigment)
                .ThenInclude(t => t.Teacher)
                .ThenInclude(t => t.User)
                .Where(t => t.Assigment != null && t.Assigment.ClassId == student.ClassId && t.DayOfWeek.ToLower().Trim() == dayToday)
                .ToListAsync();
            return schedule;
        }
        public List<SelectListItem> GetAcademicList()
        {
            return _context.AcademicYear.Select(a => new SelectListItem
            {
                Value = a.Id,
                Text = a.Name
            }).ToList();
        }
        public List<SelectListItem> GetTermList()
        {
            return _context.Term.Select(a => new SelectListItem
            {
                Value = a.Id,
                Text = a.Name
            }).ToList();
        }
        public async Task<List<SelectListItem>> GetClasses(string studentId)
        {
            return await _context.StudentsHistory
                .Where(s => s.StudentId == studentId)
                .Select(s => new SelectListItem
                {
                    Value = s.ClassId,
                    Text = s.Class.NumClass + s.Class.LetterClass
                }).ToListAsync();
        }
    }
}
