using InformationSystemOfASchoolIducationalPortal.Service;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Ученик, Родитель")]
    public class ScheduleForStudentController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;
        private readonly ScheduleForStudentLogic _scheduleLogic;
        public ScheduleForStudentController(UserManager<Users> userManager, AppDbContext context, ScheduleForStudentLogic scheduleForStudentLogic)
        {
            _context = context;
            _userManager = userManager;
            _scheduleLogic = scheduleForStudentLogic;
        }
        public async Task<IActionResult> Index(string? studentId)
        {
            Students student;
            if (string.IsNullOrWhiteSpace(studentId))
            {
                var userId = _userManager.GetUserId(User);
                student = await _context.Students.Include(u => u.User).FirstOrDefaultAsync(u => u.UserId == userId);
            }
            else
                student = await _context.Students.Include(u => u.User).FirstOrDefaultAsync(u => u.Id == studentId);
            return View(student);
        }
        public async Task<IActionResult> ScheduleLesson(string dayOfWeek, string studentId)
        {
            try
            {
                var result = await _scheduleLogic.ScheduleLesson(dayOfWeek, studentId);
                return Json(result);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "StudentPerAcc", new { error = ex.Message });
            }
        }
    }
}
