using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
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
                    id = c.Class.Id,
                    Name = c.Class.NumClass + c.Class.LetterClass,
                })
                .OrderBy(x => x.Name)
                .ToListAsync();
            return Json(classses);
        }
        public async Task<IActionResult> JournalHistory(string subjectId, string classId, string academicId, string termId)
        {
            var userId = _userMan.GetUserId(User);
            var journals = await _perAccLogic.GetListJournals(userId, subjectId, classId, academicId, termId);
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
