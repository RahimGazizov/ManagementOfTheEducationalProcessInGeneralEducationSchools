using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Ученик")]
    public class ScheduleForStudentController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;
        public ScheduleForStudentController(UserManager<Users> userManager, AppDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _context.Students.Include(u => u.User).FirstOrDefaultAsync(u => u.UserId == userId);
            return View(student);
        }
        public async Task<IActionResult> ScheduleLesson(string dayOfWeek, string studentId)
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
            return Json(scheduleList);
        }
    }
}
