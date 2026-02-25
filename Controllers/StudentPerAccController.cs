using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Ученик")]
    public class StudentPerAccController : Controller
    {
        private readonly UserManager<Users> _userMan;
        private readonly SignInManager<Users> _signIn;
        private readonly AppDbContext _context;
        public StudentPerAccController(UserManager<Users> userMan, SignInManager<Users> signIn
            , AppDbContext context)
        {
            _userMan = userMan;
            _signIn = signIn;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userMan.GetUserId(User);
                var studentClassId = await _context.Students.Where(u => u.UserId == userId).Select(c => c.ClassId)
                    .FirstOrDefaultAsync();
                var student = await _context.Students.Include(u => u.User)
                    .Include(c => c.Class)
                    .FirstOrDefaultAsync(s => s.UserId == userId);
                var listSubject = await _context.TeacherAssigments
                    .Where(t => t.ClassId == studentClassId)
                    .Include(s => s.Subject)
                    .Select(s => s.Subject)
                    .ToListAsync();
                var averageGrade = await _context.JournalEntri
                    .Where(t => t.StudentId == student.Id && t.Grade != null)
                    .AverageAsync(e => e.Grade);
                ViewBag.AverageGrade = Math.Round(averageGrade.Value, 1);
                ViewBag.ListSubject = listSubject;
                return View(student);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> JournalSet(string studentId, string subjectId, string classId, DateTime? dateFrom, DateTime? dateTo)
        {
            var journals = await _context.Journal
                 .Where(j => j.SubjectId == subjectId && j.ClassId == classId)
                 .Include(s => s.Subject)
                 .ToListAsync();
            if (dateFrom.HasValue || dateTo.HasValue)
            {
                var fromD = dateFrom?.Date;
                var toD = dateTo?.Date.AddDays(1);

                journals = journals
                    .Where(j => _context.JournalEntri.Any(it => it.JournalId == j.Id && it.StudentId == studentId &&
                    (!fromD.HasValue || it.Date >= fromD) &&
                    (!toD.HasValue || it.Date < toD))).ToList();
            }
            var journalList = journals.Select(j => new
            {
                id = j.Id,
                subjectName = j.Subject.Name,
                firstDate = _context.JournalEntri
                    .Where(it => it.JournalId == j.Id)
                    .Min(it => (DateTime?)it.Date),
                lastDate = _context.JournalEntri
                    .Where(it => it.JournalId == j.Id)
                    .Max(j => (DateTime?)j.Date),
            }).OrderByDescending(d => d.lastDate).ToList();
            return Json(journalList);
        }
        public async Task<IActionResult> JournalInfo(string id)
        {
            var journal = await _context.Journal
                .Where(j => j.Id == id)
                .Include(j => j.Entries)
                .ThenInclude(j => j.Student)
                .ThenInclude(t => t.User)
                .Include(j => j.Teacher)
                .ThenInclude(t => t.User)
                .Include(j => j.Class)
                .Include(j => j.Subject)
                .FirstOrDefaultAsync();
            return View(journal);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Authoriz");
        }
    }
}
