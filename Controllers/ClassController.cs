using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Админ")]
    public class ClassController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CRUDClass _crudClass;
        public ClassController(AppDbContext context, CRUDClass cRUDClass)
        {
            _context = context;
            _crudClass = cRUDClass;
        }
        public async Task<IActionResult> Index(string? error)
        {
            TempData["Error"] = error;
            var classes = await _context.Classes
                .Include(c => c.Students)
                .Include(s => s.AcademicYear)
                .OrderBy(n => n.NumClass)
                .ThenBy(c => c.LetterClass).ToListAsync();
            return View(classes);
        }
        [HttpPost]
        public async Task<IActionResult> AddClass(int numClass, string letterClass)
        {
            var result = await _crudClass.AddClass(numClass, letterClass);
            if (!result.Suceeced)
                return RedirectToAction("Index", new { error = result.Message });
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var _class = await _crudClass.Delete(id);
            if (_class.Suceeced)
                return RedirectToAction("Index", new { error = _class.Message });
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, int numClass, string letterClass)
        {
            var cls = await _crudClass.Edit(id, numClass, letterClass);
            if (!cls.Suceeced)
                return RedirectToAction("Index", new { error = cls.Message });
            if (!cls.Suceeced)
                return RedirectToAction("Index", new { error = cls.Message });
            return RedirectToAction("Index");
        }
    }
}
