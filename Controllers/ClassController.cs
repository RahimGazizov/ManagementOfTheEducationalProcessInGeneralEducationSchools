using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class ClassController : Controller
    {
        private readonly AppDbContext _context;
        public ClassController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? error)
        {
            TempData["Error"] = error;
            var classes = await _context.Classes.Include(c => c.Students).ToListAsync();
            return View(classes);
        }
        [HttpPost]
        public async Task<IActionResult> AddClass(int numClass, string letterClass)
        {
            if (_context.Classes.FirstOrDefault(c => c.LetterClass == letterClass && c.NumClass == numClass) != null)
                return RedirectToAction("Index", new { error = "Такой класс уже существует" });
            var classes = new Class
            {
                NumClass = numClass,
                LetterClass = letterClass
            };
            await _context.Classes.AddAsync(classes);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var _class = await _context.Classes.FirstOrDefaultAsync(i => i.Id == id);
            if (_class == null)
                return RedirectToAction("Index", new { error = "Пользователь не найден" });
            _context.Classes.Remove(_class);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, int numClass, string letterClass)
        {
            var cls = await _context.Classes.FindAsync(id);
            if (cls == null)
                return RedirectToAction("Index", new { error = "Пользователь не найден" });
            if (_context.Classes.FirstOrDefault(c => c.LetterClass == letterClass && c.NumClass == numClass) != null)
                return RedirectToAction("Index", new { error = "Такой класс уже существует" });
            cls.NumClass = numClass;
            cls.LetterClass = letterClass;
            _context.Classes.Update(cls);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
