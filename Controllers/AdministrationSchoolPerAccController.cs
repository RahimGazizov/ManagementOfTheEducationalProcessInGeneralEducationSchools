using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "АдминистрацияШколы")]
    public class AdministrationSchoolPerAccController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;
        private readonly SendEmailService _sendEmail;
        private readonly AnaliticalService _analiticalService;
        public AdministrationSchoolPerAccController(UserManager<Users> userManager, AppDbContext context,
            SendEmailService sendEmail, AnaliticalService analiticalService)
        {
            _userManager = userManager;
            _context = context;
            _sendEmail = sendEmail;
            _analiticalService = analiticalService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _context.Administrations
                .Include(s => s.User)
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();
            var currentYear = await _context.AcademicYear
    .FirstOrDefaultAsync(d => DateTime.Today >= d.StartDateYear
                           && DateTime.Today <= d.EndDateYear);
            var academicList = await _context.AcademicYear
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name,
                    Selected = currentYear != null && currentYear.Id == s.Id
                })
                .ToListAsync();
            var modelView = new AdministrationModelView
            {
                Administration = user,
                AcademicYears = academicList
            };
            return View(modelView);
        }
        public async Task<IActionResult> GetDataByAcademinYear(string academicId)
        {
            if (string.IsNullOrWhiteSpace(academicId))
                return BadRequest(new { message = "Айди учебного года пуст" });
            var exists = await _context.AcademicYear.AnyAsync(a => a.Id == academicId);
            if (!exists)
                return NotFound(new { message = "Учебный год по айди не найден" });
            var classList = await _context.Classes
                .Where(s => s.AcademicYearId == academicId)
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.NumClass + "-" + s.LetterClass
                })
                .ToListAsync();
            var termList = await _context.Term
                .Where(s => s.AcademicYearId == academicId)
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                })
                .ToListAsync();

            var subjectList = await _context.Subjects
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.Name
                }).ToListAsync();

            return Json(new
            {
                classes = classList,
                terms = termList,
                subjects = subjectList
            });
        }
        public async Task<IActionResult> GetStudentsByClass(string classId)
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId)
                .Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.User.FullName
                })
                .ToListAsync();
            return Json(students);
        }
        public async Task<IActionResult> AnaliticalReport(
       string academicId,
       string termId,
       string classId,
       string subjectId,
       string? studentId)
        {
            var result = await _analiticalService.AnaliticalData(academicId, termId, classId, subjectId, studentId);

            return View(result.Data);
        }
        public async Task<IActionResult> SendEmail(string studentId, string academicId, string termId,
                  string classId,
                  string subjectId)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"StudentID-{studentId}");
            Console.WriteLine($"academicId-{academicId}");
            Console.WriteLine($"termId-{termId}");
            Console.WriteLine($"classId-{classId}");
            Console.WriteLine($"subjectId-{subjectId}");
            Console.ForegroundColor = ConsoleColor.White;

            if (string.IsNullOrWhiteSpace(studentId))
                return BadRequest(new { message = "Айди студента пуст" });
            var student = await _context.Students
                .Include(s => s.Parents)
                .ThenInclude(s => s.User)
                .Where(s => s.Id == studentId)
                .FirstOrDefaultAsync();
            if (student == null)
                return BadRequest(new { message = "Студент не найден" });
            
            var emails = student.Parents
                .Where(p => !string.IsNullOrWhiteSpace(p.Email))
                .Select(p => new
            {
                EmailParent = p.Email,
                ParentName = p.User.FullName,

            }).ToList();
            if (!emails.Any())
                return BadRequest(new { message = "У родителей нету email" });
            var dataStudents = await _analiticalService.AnaliticalData(academicId, termId, classId, subjectId, studentId);
            var data = dataStudents.Data.DataStudents.FirstOrDefault();
            if (data == null)
                return BadRequest(new { message = "Нет данных по студенту" });
            var results = new List<EmailSendResult>();
            foreach (var email in emails)
            {
                var message = $@"
Уважаемая(ый) {email.ParentName},

Администрация школы уведомляет вас о том, что у вашего ребенка наблюдается низкий уровень успеваемости.

Информация об ученике:
ФИО: {data.StudentFullName}
Класс: {dataStudents.Data.ClassName}
Предмет: {dataStudents.Data.SubjectName}

Показатели:
Средний балл: {data.AverageGrade}
Посещаемость: {data.PercentOfPresence}%
Индекс успеваемости: {data.AcademicPerformanceIndex}

С уважением,
Администрация школы";
                var result = await _sendEmail.SendEmail(email.EmailParent, email.ParentName, "Успевамость ребенка", message);
                results.Add(new EmailSendResult
                {
                    Email = email.EmailParent,
                    Success = result.Success,
                    Message = result.Message
                });

            }
            return Ok(new
            {
                results
            });

        }
        public class EmailSendResult
        {
            public string Email { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        }
    }
}
