using InformationSystemOfASchoolIducationalPortal.Service;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Админ")]
    public class SubjectsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CRUDSubject _crudSub;
        public SubjectsController(AppDbContext context, CRUDSubject cRUD)
        {
            _context = context;
            _crudSub = cRUD;
        }
        public async  Task<IActionResult> Index()
        {
            var sub = await _context.Subjects.OrderBy(n => n.Name).ToListAsync();
            return View(sub);
        }
        public async Task<IActionResult> AddSubject(string subjectName)
        {
           var result = await _crudSub.Add(subjectName);
            if(!result.Success)
            {
                TempData["Error"] = result.Message;
                TempData["SubjectName"] = subjectName;
                return View("Index", await _context.Subjects.ToListAsync());
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _crudSub.Delete(id);
            if(!result.Success)
                TempData["ErrorMessage"] = "Предмет не найден";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditSubject(string id, string subjectName)
        {
            var result = await _crudSub.Edit(id, subjectName);
            if (!result.Success)
            {
                TempData["EditError"] = result.Message;
                TempData["IdSub"] = id;
                TempData["editSub"] = subjectName;
                return RedirectToAction("Index", await _context.Subjects.ToListAsync());
            }
            return RedirectToAction("Index");
        }
    }
}
