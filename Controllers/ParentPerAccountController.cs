using InformationSystemOfASchoolIducationalPortal.Service;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Родитель")]
    public class ParentPerAccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;
        private readonly SignInManager<Users> _signIn;
        private readonly ParentPerAccLogic _parentLogic;
        public ParentPerAccountController(UserManager<Users> userManager, AppDbContext context, SignInManager<Users> signIn,
            ParentPerAccLogic parentPerAccLogic)
        {
            _userManager = userManager;
            _context = context;
            _signIn = signIn;
            _parentLogic = parentPerAccLogic;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var parent = await _context.Parent.Include(u => u.User).FirstOrDefaultAsync(u => u.UserId == userId);
            var result = await _parentLogic.ForIndexData(parent);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(result);
            }
            return View(result.Data);
        }
        public async Task<IActionResult> ClassList(string studentId)
        {
            var classes = await _context.StudentsHistory
                .Include(s => s.Class)
                .Where(s => s.StudentId == studentId)
                .Select(s => new
                {
                    id = s.ClassId,
                    name = s.Class.NumClass + s.Class.LetterClass
                }).ToListAsync();
            return Json(classes);
        }
        public async Task<IActionResult> SubjectList(string classId)
        {
            Console.WriteLine();
            var subjects = await _context.TeacherAssigments
                .Include(s => s.Subject)
                .Where(s => s.ClassId == classId)
                .Select(s => new
                {
                    id = s.SubjectId,
                    name = s.Subject.Name,
                })
                .ToListAsync();
            var academic = await _context.StudentsHistory
               .Include(s => s.AcademicYear)
               .Where(s => s.ClassId == classId)
               .Select(s => new
               {
                   id = s.AcademicYearId,
                   name = s.AcademicYear.Name,
               })
               .FirstOrDefaultAsync();
            var terms = await _context.Term
                .Where(s => s.AcademicYearId == academic.id)
                .Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                })
                .ToListAsync();
            return Json(new
            {
                subjects,
                academic,
                terms
            });
        }
        public async Task<IActionResult> ScheduleStudentForParent(string id)
        {
            var student = await _context.Students
                .Where(s => s.Id == id).FirstOrDefaultAsync();
            if (student == null)
            {
                var userId = _userManager.GetUserId(User);
                var parent = await _context.Parent.Include(u => u.User).FirstOrDefaultAsync(u => u.UserId == userId);
                var modal = await _parentLogic.ForIndexData(parent);
                modal.Data.IsSchedule = true;
                return RedirectToAction("Index", new { modal });
            }
            return View(student);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Authoriz");
        }
    }
}
