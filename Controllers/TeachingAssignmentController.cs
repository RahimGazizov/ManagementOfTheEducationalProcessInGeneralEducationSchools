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
            var exists = await _assigmentLogic.Add(assigment);
            if (!exists.Success)
            {
                TempData["Error"] = exists.Message;
                ViewBag.ListTeachers = await _assigmentLogic.GetListTeachers();
                ViewBag.ListSubjects = await _assigmentLogic.GetListSubjects();
                return View(assigment);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(string id)
        {
            var assigment = await GetTeachingAssigment(id);
            if (assigment == null)
            {
                TempData["Error"] = "Запись не найдена";
                return RedirectToAction("Index");
            }
            ViewBag.ListTeachers = await _assigmentLogic.GetListTeachers();
            ViewBag.ListSubjects = await _assigmentLogic.GetListSubjects();
            return View(assigment);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TeacherAssigment assigment)
        {
            var findAssigment = await _assigmentLogic.Edit(assigment);
            if (!findAssigment.Success)
            {
                TempData["Error"] = findAssigment.Message;
                ViewBag.ListTeachers = await _assigmentLogic.GetListTeachers();
                ViewBag.ListSubjects = await _assigmentLogic.GetListSubjects();
                var assig = await GetTeachingAssigment(assigment.Id);
                return View(assig);
            }
            return RedirectToAction("Index");
        }
        private async Task<TeacherAssigment> GetTeachingAssigment(string id)
        {
            return await _context.TeacherAssigments
         .Include(t => t.Teacher)
             .ThenInclude(u => u.User)
         .Include(t => t.Subject)
         .Include(t => t.Class)
         .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
