using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.Service;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Учитель")]
    public class TeacherPerAccController : Controller
    {
        private readonly UserManager<Users> _userMan;
        private readonly SignInManager<Users> _signIn;
        private readonly AppDbContext _context;
        private readonly TeacherPerAccLogic _perAccLogic;
        private readonly ActionLogService _actionLogService;
        public TeacherPerAccController(UserManager<Users> userMan, AppDbContext context
            , SignInManager<Users> signIn, TeacherPerAccLogic perAccLogic, ActionLogService actionLogService)
        {
            _userMan = userMan;
            _context = context;
            _signIn = signIn;
            _perAccLogic = perAccLogic;
            _actionLogService = actionLogService;
        }
        public async Task<IActionResult> Index(string? error)
        {
            TempData["Error"] = error;
            var userId = _userMan.GetUserId(User);
            var teacher = await _context.Teachers.Include(u => u.User)
                .FirstOrDefaultAsync(id => id.UserId == userId);
            var currentYear = await _context.AcademicYear
                .Where(d => d.StartDateYear <= DateTime.Now && d.EndDateYear >= DateTime.Now)
                .FirstOrDefaultAsync();
            var currentTerm = await _context.Term.
                Where(t => t.DateStartTerm <= DateTime.Now && t.DateEndTerm >= DateTime.Now)
                .FirstOrDefaultAsync();
            ViewBag.CurrentTerm = currentTerm;
            ViewBag.CurrentYear = currentYear;
            ViewBag.ListAcademicYear = GetListAcademicYear();
            ViewBag.ListTerms = GetListTerms();
            ViewBag.ListSubjects = await _perAccLogic.GetListSubjects(userId);
            return View(teacher);
        }
        public async Task<IActionResult> GetClassesBySubject(string subjectId)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Subject {subjectId}");
            Console.ForegroundColor = ConsoleColor.White;

            string currentAcademic = await _context.AcademicYear
                .Where(d => DateTime.Now >= d.StartDateYear && DateTime.Now <= d.EndDateYear)
                .Select(d => d.Id)
                .FirstOrDefaultAsync() ?? "";
            if (string.IsNullOrWhiteSpace(currentAcademic))
            {
                if (currentAcademic == null)
                    return View("Index", new { error = "Учебный год не создан" });
            }
            var userId = _userMan.GetUserId(User);
            var teacher = await _context.Teachers.Include(u => u.User)
                .FirstOrDefaultAsync(id => id.UserId == userId);
            var classses = await _context.TeacherAssigments
                .Where(r => r.TeacherId == teacher.Id && r.SubjectId == subjectId && r.AcademicId == currentAcademic)
                .Select(c => new
                {
                    id = c.Class.Id,
                    Name = c.Class.NumClass + c.Class.LetterClass,
                })
                .OrderBy(x => x.Name)
                .ToListAsync();
            return Json(classses);
        }
        public async Task<IActionResult> JournalHistory(string subjectId, string classId)
        {
            var userId = _userMan.GetUserId(User);
            string currentAcademic = await _context.AcademicYear
                .Where(d => DateTime.Now >= d.StartDateYear && DateTime.Now <= d.EndDateYear)
                .Select(d => d.Id)
                .FirstOrDefaultAsync() ?? "";
            string currentTerm = await _context.Term
                .Where(d => d.AcademicYearId == currentAcademic && d.DateStartTerm <= DateTime.Now && DateTime.Now <= d.DateEndTerm)
                .Select(d => d.Id)
                .FirstOrDefaultAsync() ?? "";
            var journals = await _perAccLogic.GetListJournals(userId, subjectId, classId, currentAcademic, currentTerm);
            var journalList = journals
    .Select(j => new
    {
        id = j.Id,
        date = j.Date,
    })
    .OrderByDescending(x => x.date)
    .ToList();
            return Json(journalList);
        }
        public IActionResult ListJournal()
        {
            var journal = _context.Journal
                .Include(j => j.Subject)
                .Include(j => j.Class)
                .Include(j => j.Teacher)
                .ThenInclude(j => j.User)
                .Include(j => j.Entries).ToList();
            return View(journal);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            await _actionLogService.LogAsync(
               "Выход пользователя",
               "User",
               null,
               "Учитель вышел с системы"
               );
            return RedirectToAction("Index", "Authoriz");
        }
        private List<SelectListItem> GetListAcademicYear()
        {
            return _context.AcademicYear.Select(a => new SelectListItem
            {
                Value = a.Id,
                Text = a.Name
            }).ToList();
        }
        private List<SelectListItem> GetListTerms()
        {
            return _context.Term.Select(a => new SelectListItem
            {
                Value = a.Id,
                Text = a.Name
            }).ToList();
        }
    }
}
