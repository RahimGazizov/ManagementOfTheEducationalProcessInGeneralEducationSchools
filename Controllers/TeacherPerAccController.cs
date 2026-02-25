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
        //    var entri = await _context.JournalEntri.ToListAsync();
        //    _context.RemoveRange(entri);
        //    var journal = await _context.Journal.ToListAsync();
        //    _context.RemoveRange(journal);
        //    await _context.SaveChangesAsync();
            var userId = _userMan.GetUserId(User);
            var teacher = await _context.Teachers.Include(u => u.User)
                .FirstOrDefaultAsync(id => id.UserId == userId);
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
        public async Task<IActionResult> JournalHistory(string subjectId, string classId, DateTime? dateFrom, DateTime? dateTo)
        {
            var userId = _userMan.GetUserId(User);
            var journals = await _perAccLogic.GetListJournals(userId, subjectId, classId, dateFrom, dateTo);
            var journalList = journals
                .Select(j => new
                {
                    id = j.Id,
                    firstDate = _context.JournalEntri
                    .Where(it => it.JournalId == j.Id)
                    .Min(it => (DateTime?)it.Date),
                    lastDate = _context.JournalEntri
                    .Where(it => it.JournalId == j.Id)
                    .Max(j => (DateTime?)j.Date),
                    markCount = _context.JournalEntri.Count(j => j.Id == j.Id)
                }).OrderByDescending(j => j.lastDate)
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
    }
}
