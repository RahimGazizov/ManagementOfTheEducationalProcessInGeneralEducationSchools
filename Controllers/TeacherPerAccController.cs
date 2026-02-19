using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Учитель")]
    public class TeacherPerAccController : Controller
    {
        private readonly UserManager<Users> _userMan;
        private readonly SignInManager<Users> _signIn;
        private readonly AppDbContext _context;
        private readonly TeacherPerAccLogic _perAccLogic;
        public TeacherPerAccController(UserManager<Users> userMan, AppDbContext context
            , SignInManager<Users> signIn, TeacherPerAccLogic perAccLogic)
        {
            _userMan = userMan;
            _context = context;
            _signIn = signIn;
            _perAccLogic = perAccLogic;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userMan.GetUserId(User);
            var teacher = await _context.Teachers.Include(u => u.User)
                .FirstOrDefaultAsync(id => id.UserId == userId);
            var teacherId = await _context.Teachers
                .Where(t => t.UserId == userId)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();
            ViewBag.ListSubjects = await _perAccLogic.GetListSubjects(teacherId);
            return View(teacher);
        }
        public async Task<IActionResult> GetClassesBySubject(string subjectId)
        {
            var userId = _userMan.GetUserId(User);
            var teacher = await _context.Teachers.Include(u => u.User)
                .FirstOrDefaultAsync(id => id.UserId == userId);
            var teacherId = await _context.Teachers
                .Where(t => t.UserId == userId)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();
            var classses = await _context.TeacherAssigments
                .Where(r => r.TeacherId == teacherId && r.SubjectId == subjectId)
                .Select(c => new
                {
                    Id = c.Class.Id,
                    Name = c.Class.NumClass + c.Class.LetterClass,
                })
                .OrderBy(x => x.Name)
                .ToListAsync();
            return Json(classses);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Authoriz");
        }
    }
}
