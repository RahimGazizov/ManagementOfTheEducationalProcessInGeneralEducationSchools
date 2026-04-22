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
            var entries = await _context.Journal
                .Where(j => j.ClassId == classId && j.SubjectId == subjectId
                && j.AcademicYearId == academicId && j.TermId == termId)
                .SelectMany(j => j.Entries)
                .Where(j => j.StudentId == studentId)
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
        public async Task<IActionResult> GetClassRating(string studentid, string classId, string academicId, string termId)
        {
            var data = await _context.JournalEntry
                .Where(s => s.Journal.ClassId == classId
                    && s.Journal.AcademicYearId == academicId
                    && s.Journal.TermId == termId)
                .GroupBy(s => new
                {
                    s.StudentId,
                    s.Student.User.FullName
                })
                .Select(g => new
                {
                    StudentsId = g.Key.StudentId,
                    StudentName = g.Key.FullName,
                    AvgGrade = Math.Round(g.Where(s => s.Grade != null)
                                .Average(s => (double?)s.Grade) ?? 0, 1),
                    LessonCount = g.Count(),
                    PresentLesson = g.Count(s => s.IsPresent)
                })
                .ToListAsync();

            var rating = data
                .Select(s =>
                {
                    double attendancePercent = s.LessonCount == 0
                        ? 0
                        : (double)s.PresentLesson / s.LessonCount * 100;

                    double score = 0.8 * s.AvgGrade + 0.2 * (attendancePercent / 20.0);

                    return new
                    {
                        s.StudentsId,
                        s.StudentName,
                        s.AvgGrade,

                        AttendancePercent = Math.Round(attendancePercent, 1),
                        Score = Math.Round(score, 1)
                    };
                })
                .OrderByDescending(s => s.Score)
                .ThenByDescending(s => s.AvgGrade)
                .ToList();
            var result = rating
                .Select((x, index) => new
                {
                    place = index + 1,
                    x.StudentsId,
                    x.StudentName,
                    x.AvgGrade,
                    x.AttendancePercent,
                    x.Score
                }).ToList();

            var top3 = result.Take(3).ToList();
            var currentUserRating = result.FirstOrDefault(x => x.StudentsId == studentid);
            var ratingParallel = await GetParallelRating(studentid, classId, academicId, termId);
            return Json(new
            {
                top3,
                currentUserRating,
                totalStudent = result.Count,
                ratingParallel
            });

        }
        public async Task<object> GetParallelRating(string studentid, string classId, string academicId, string termId)
        {
            try
            {

                var numClass = await _context.Classes
                    .Where(s => s.Id == classId).Select(s => s.NumClass).FirstOrDefaultAsync();

                var data = await _context.JournalEntry
                    .Where(s => s.Journal.Class.NumClass == numClass
                    && s.Journal.AcademicYearId == academicId
                    && s.Journal.TermId == termId)
                    .GroupBy(s => new
                    {
                        s.StudentId,
                        s.Student.User.FullName,
                    })
                    .Select(s => new
                    {
                        StudentsId = s.Key.StudentId,
                        StudentFullName = s.Key.FullName,
                        ClassNum = s.Select(g => g.Journal.Class.NumClass).FirstOrDefault(),
                        ClassLetter = s.Select(g => g.Journal.Class.LetterClass).FirstOrDefault(),
                        AvgGrade = Math.Round(s.Where(g => g.Grade != null)
                        .Average(g => g.Grade) ?? 0, 1),
                        LessonCount = s.Count(),
                        PresentLesson = s.Count(g => g.IsPresent)
                    }).ToListAsync();
                var rating = data
                   .Select(s =>
                   {
                       double attendancePercent = s.PresentLesson == 0 ? 0 : (double)s.PresentLesson / s.LessonCount * 100;
                       double score = 0.8 * s.AvgGrade + 0.2 * (attendancePercent / 20.0);
                       return new
                       {
                           s.StudentsId,
                           s.StudentFullName,
                           s.AvgGrade,
                           s.ClassNum,
                           s.ClassLetter,
                           AttendancePercent = Math.Round(attendancePercent, 1),
                           Score = Math.Round(score, 1)
                       };
                   })
                   .OrderByDescending(s => s.Score)
                   .ThenByDescending(s => s.AvgGrade)
                   .ToList();
                var result = rating
                    .Select((s, index) => new
                    {
                        place = index + 1,
                        s.StudentsId,
                        s.StudentFullName,
                        s.AvgGrade,
                        s.AttendancePercent,
                        s.Score,
                        s.ClassNum,
                        s.ClassLetter
                    });
                var top3Parallel = result.Take(3).ToList();
                var currentUser = result.FirstOrDefault(s => s.StudentsId == studentid);
                return new
                {
                    top3Parallel,
                    currentUser,
                    totalStudentParallel = result.Count(),
                };
            }
            catch (Exception ex)
            {
                return View("Index", new { error = ex });
            }

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
