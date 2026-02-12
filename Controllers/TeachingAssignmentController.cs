using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using Microsoft.AspNetCore.Authorization;
namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Админ")]
    public class TeachingAssignmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly TeachingAssigmentLogic _assigmentLogic;
        public TeachingAssignmentController(AppDbContext context, TeachingAssigmentLogic assigmentLogic)
        {
            _context = context;
            _assigmentLogic = assigmentLogic;
        }
        public async Task<IActionResult> Index()
        {

            var techingsAssigment = await _context.TeacherAssigments
                .Include(t => t.Teacher)
                .ThenInclude(t => t.User)
                .Include(c => c.Class)
                .Include(s => s.Subject)
                .ToListAsync();
            return View(techingsAssigment);
        }
        public async Task<IActionResult> Add()
        {
            ViewBag.ListTeachers = await _assigmentLogic.GetListTeachers();
            ViewBag.ListSubjects = await _assigmentLogic.GetListSubjects();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(TeacherAssigment assigment)
        {
            Console.WriteLine($"Teacherid-{assigment.TeacherId}\nSubjectId-{assigment.SubjectId}\nClassId-{assigment.ClassId}");
            var exists = await _context.TeacherAssigments
               .AnyAsync(a => a.TeacherId == assigment.TeacherId &&
               a.SubjectId == assigment.SubjectId && a.ClassId == assigment.ClassId);
            if (exists)
            {
                TempData["Error"] = "Такая сущность уже есть";
                ViewBag.ListTeachers = await _assigmentLogic.GetListTeachers();
                ViewBag.ListSubjects = await _assigmentLogic.GetListSubjects();
                return View(assigment);
            }
            var newAssigment = new TeacherAssigment
            {
                TeacherId = assigment.TeacherId,
                SubjectId = assigment.SubjectId,
                ClassId = assigment.ClassId,
            };
            _context.TeacherAssigments.Add(newAssigment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
