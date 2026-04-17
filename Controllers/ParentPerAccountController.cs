using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class ParentPerAccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;
        private readonly SignInManager<Users> _signIn;
        public ParentPerAccountController(UserManager<Users> userManager, AppDbContext context, SignInManager<Users> signIn)
        {
            _userManager = userManager;
            _context = context;
            _signIn = signIn;
        }
        [Authorize(Roles = "Родитель")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var parent = await _context.Parent.Include(u => u.User).FirstOrDefaultAsync(u => u.UserId == userId);
            var parentWithStudents = await _context.Parent
     .Include(p => p.Students)
     .ThenInclude(s => s.User)
     .Include(s => s.Students)
     .ThenInclude(s => s.Class)
     .FirstOrDefaultAsync(p => p.Id == parent.Id);

            var children = parentWithStudents?.Students?
      .Select(s => new SelectListItem
      {
          Value = s.Id,
          Text = s.User.FullName
      })
      .ToList() ?? new List<SelectListItem>();
            List<StudentAvgScore> avgScores = new();
            var childIds = parentWithStudents.Students.Select(s => s.Id).ToList();
            var entries = await _context.JournalEntry
                .Include(s => s.Journal)
                .Where(s => childIds.Contains(s.StudentId))
                .ToListAsync();
            foreach (var child in parentWithStudents.Students)
            {
                var childGrades = entries
         .Where(e => e.StudentId == child.Id &&
                     e.Journal.ClassId == child.ClassId &&
                     e.Grade != null)
         .Select(e => e.Grade);

                double avg = childGrades.Any() ? childGrades.Average() ?? 0 : 0;

                avgScores.Add(new StudentAvgScore
                {
                    Student = child,
                    AvgScore = avg
                });
            }
            var viewModel = new ModelViewForParents
            {
                Parent = parent,
                Childrens = children,
                Students = avgScores
            };
            return View(viewModel);
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
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Authoriz");
        }
    }
}
