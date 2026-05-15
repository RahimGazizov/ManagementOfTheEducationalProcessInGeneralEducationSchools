using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static InformationSystemOfASchoolIducationalPortal.Service.StudentPerAccLogic;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Ученик, Родитель")]
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
        public async Task<IActionResult> Index(string? error)
        {
            try
            {
                TempData["Error"] = error;
                var userId = _userMan.GetUserId(User);
                var student = await _context.Students.Include(u => u.User)
                    .Include(c => c.Class)
                    .FirstOrDefaultAsync(s => s.UserId == userId);
                var studentDash = await ViewBagList(student);
                return View(studentDash);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var userId = _userMan.GetUserId(User);
                var student = await _context.Students.Include(u => u.User)
                     .Include(c => c.Class)
                     .FirstOrDefaultAsync(s => s.UserId == userId);
                var studentDashBoard = new StudentDashboardViewModel
                {
                    Student = student
                };
                return View(studentDashBoard);
            }
        }
        public IActionResult GraphShow(string studentId, string classId, string acdemicId, string termId) => View();
        public async Task<IActionResult> SubjectList(string studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            var subjects = await _context.TeacherAssigments
                .Include(s => s.Subject)
                .Where(s => s.ClassId == student.ClassId && DateTime.Now >= s.AcademicYear.StartDateYear
                && DateTime.Now <= s.AcademicYear.EndDateYear)
                .Select(s => s.Subject)
                .ToListAsync();
            var list = subjects.Select(s => new
            {
                id = s.Id,
                name = s.Name
            });
            return Json(list);
        }
        [HttpGet]
        public async Task<IActionResult> JournalSet(string studentId, string subjectId)
        {
            if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(subjectId))
                return BadRequest(new { message = "studentId или subjectId пустой" });

            var currentAcademic = await _context.AcademicYear
                .FirstOrDefaultAsync(d => DateTime.Now >= d.StartDateYear && DateTime.Now <= d.EndDateYear);

            if (currentAcademic == null)
                return BadRequest(new { message = "Учебный год не был добавлен" });

            var currentTerm = await _context.Term
                .FirstOrDefaultAsync(d => d.AcademicYearId == currentAcademic.Id &&
                DateTime.Today >= d.DateStartTerm && DateTime.Today <= d.DateEndTerm);
            Console.WriteLine($"TermName - {currentTerm.Name}");
            if (currentTerm == null)
                return BadRequest(new { message = "Четверть не была добавлена" });

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return NotFound(new { message = "Студент не найден" });

            if (student.ClassId == null)
                return BadRequest(new { message = "Студенту не назначен класс" });

            var journalList = await _context.Journal
                .Where(j => j.SubjectId == subjectId
                    && j.ClassId == student.ClassId
                    && j.AcademicYearId == currentAcademic.Id
                    && j.TermId == currentTerm.Id)
                .Include(j => j.Subject)
                .OrderByDescending(j => j.Date)
                .Select(j => new
                {
                    id = j.Id,
                    subjectName = j.Subject.Name,
                    date = j.Date
                })
                .ToListAsync();

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
            var result = await _studentPer.AvgGradeForTheSubject(classId, subjectId, academicId, termId, studentId);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                var userId = _userMan.GetUserId(User);
                var student = await _context.Students.Include(u => u.User)
                    .Include(c => c.Class)
                    .FirstOrDefaultAsync(s => s.UserId == userId);
                var studentDash = await ViewBagList(student);
                return View(studentDash);
            }
            return Json(new
            {
                average = result.Data.Average,
                name = result.Data.Name,
                percent = result.Data.Percent
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
        public async Task<IActionResult> GetClassRating(string studentid, string classId, string academicId, string termId)
        {
            var result = await _studentPer.RatingStudent(studentid, classId, academicId, termId);
            if (!result.Success)
            {
                return Json(new {
                     success = false,
                     message = result.Message
                });
            }
            return Json(new
            {
                success = true,
                ratingCurrentClass = result.Data.RatingCurrentClass,
                ratingParallelClass = result.Data.RatingParallelClass
            });

        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Authoriz");
        }

        private async Task<StudentDashboardViewModel> ViewBagList(Students student)
        {
            var studentDash = new StudentDashboardViewModel
            {
                Student = student,
                CurrentTerm = await _studentPer.GetCurrentTerm(),
                CurrentYear = await _studentPer.GetCurrentAcademicYear(),
                AverageGrade = Math.Round(await _studentPer.AverageGrade(student), 1),
                Subjects = await _studentPer.ListSubjects(student.ClassId),
                Schedule = await _studentPer.ScheduleLessons(student),
                AcademicYears = _studentPer.GetAcademicList(),
                Terms = _studentPer.GetTermList(),
                Classes = await _studentPer.GetClasses(student.Id),
                Delta = await _studentPer.AvgScoreDimamics(student)
            };
            return studentDash;

        }
    }
}
