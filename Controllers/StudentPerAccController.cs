using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Ученик")]
    public class StudentPerAccController : Controller
    {
        private readonly UserManager<Users> _userMan;
        private readonly SignInManager<Users> _signIn;
        private readonly AppDbContext _context;
        private readonly StudentPerAccLogic _studentPer;
        public StudentPerAccController(UserManager<Users> userMan, SignInManager<Users> signIn
            , AppDbContext context, StudentPerAccLogic studentPerAcc)
        {
            _userMan = userMan;
            _signIn = signIn;
            _context = context;
            _studentPer = studentPerAcc;
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
                var studentDash = await ViewBagList(studentClassId, student);
                return View(studentDash);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var userId = _userMan.GetUserId(User);
                var student = await _context.Students.Include(u => u.User)
                     .Include(c => c.Class)
                     .FirstOrDefaultAsync(s => s.UserId == userId);
                return View(student);
            }
        }
        public IActionResult GraphShow(string studentId, string classId, string acdemicId, string termId) => View();
        [HttpGet]
        public async Task<IActionResult> JournalSet(string studentId, string subjectId, string classId, string academicId, string termId)
        {
            var journals = await _context.Journal
                 .Where(j => j.SubjectId == subjectId && j.ClassId == classId &&
                 j.AcademicYearId == academicId && j.TermId == termId)
                 .Include(s => s.Subject)
                 .Include(s => s.AcademicYear)
                 .Include(s => s.Term)
                 .ToListAsync();

            var journalList = journals.Select(j => new
            {
                id = j.Id,
                subjectName = j.Subject.Name,
                date = j.Date
            }).OrderByDescending(x => x.date).ToList();

            return Json(journalList);
        }
        [HttpGet]
        public async Task<IActionResult> GetClassData(string classId)
        {
            var subjects = await _studentPer.ListSubjects(classId);

            var academicYearId = await _context.StudentsHistory
                .Where(c => c.ClassId == classId)
                .Select(c => c.AcademicYearId)
                .FirstOrDefaultAsync();

            var academicYear = await _context.AcademicYear
                .FirstOrDefaultAsync(a => a.Id == academicYearId);

            var terms = await _context.Term
                .Where(t => t.AcademicYearId == academicYearId)
                .ToListAsync();

            return Json(new
            {
                academicYear = academicYear != null ? new { academicYear.Id, academicYear.Name } : null,
                subjects = subjects.Select(s => new { s.Id, s.Name }),
                terms = terms.Select(t => new { t.Id, t.Name })
            });
        }
        public async Task<IActionResult> GetResultInfo(string classId, string subjectId, string academicId, string termId, string studentId)
        {
            var entries = await _context.Journal
                .Where(j => j.ClassId == classId && j.SubjectId == subjectId
                && j.AcademicYearId == academicId && j.TermId == termId)
                .SelectMany(j => j.Entries)
                .Where(j => j.StudentId ==  studentId)
                .ToListAsync();
            var averageScore = entries.Any() ? entries.Average(j => j.Grade) : 0;
            var subjectName = await _context.Subjects.Where(s => s.Id == subjectId).Select(s => s.Name).FirstOrDefaultAsync();
            var count = entries.Count(s => s.IsPresent);
            var percentageOfAttendance = entries.Any() ? (double)count / entries.Count * 100 : 0;
            return Json(new
            {
                average = averageScore,
                name = subjectName,
                percent = percentageOfAttendance
            });
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
        public async Task<IActionResult> GetQuarterStats(string studentId, string classId, string academicId, string termId)
        {
            var data = await _context.JournalEntry
                .Where(d => d.StudentId == studentId 
                && d.Journal.ClassId == classId && d.Journal.AcademicYearId == academicId 
                && d.Journal.TermId == termId
                && d.Grade != null)
                .GroupBy(d => d.Journal.Subject.Name)
                .ToListAsync();
            var result = data.Select(s => new
            {
                subject = s.Key,
                average = s.Average(s => s.Grade)
            });
            return Json(result);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Authoriz");
        }
        private async Task<StudentDashboardViewModel> ViewBagList(string studentClassId, Students student)
        {
            var studentDash = new StudentDashboardViewModel
            {
                Student = student,
                CurrentTerm = await _studentPer.GetCurrentTerm(),
                CurrentYear = await _studentPer.GetCurrentAcademicYear(),
                AverageGrade = Math.Round(await _studentPer.AverageGrade(student), 1),
                Subjects = await _studentPer.ListSubjects(studentClassId),
                Schedule = await _studentPer.ScheduleLessons(studentClassId),
                AcademicYears = _studentPer.GetAcademicList(),
                Terms = _studentPer.GetTermList(),
                Classes = await _studentPer.GetClasses(student.Id)
            };
            return studentDash;
        }
    }
}
