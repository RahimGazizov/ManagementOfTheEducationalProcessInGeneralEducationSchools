using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Админ")]
    public class ParentsController : Controller
    {
        private readonly AppDbContext _context;
        public ParentsController(AppDbContext context)
        {
            _context = context;
        }
        // GET: ParentsController
        public async Task<IActionResult> Index()
        {
            var parents = await _context.Parent
    .Include(p => p.User)
    .Include(p => p.Students) 
        .ThenInclude(s => s.User) 
        .Include(s => s.Students)
        .ThenInclude(s => s.Class)
    .ToListAsync();
            var parentView = new ParentsViewModel
            {
                Parent = parents,
            };
            return View(parentView);
        }

        public async Task<IActionResult> GetLetterClass(int numClass)
        {
            var classList = await _context.Classes
                .Where(s => s.NumClass == numClass).ToListAsync();

            var letters = classList.
                Select(s => new
                {
                    id = s.Id,
                    letter = s.LetterClass,
                });
            return Json(letters);
        }
        public async Task<IActionResult> GetNameStudents(string classId)
        {
            var students = await _context.Students
                .Include(u => u.User)
                .Where(s => s.ClassId == classId).ToListAsync();
            var listStudents = students
                .Select(s => new
                {
                    id = s.Id,
                    name = s.User.FullName,
                });
            return Json(listStudents);
        }
        public async Task<IActionResult> AddChild(string parentId, string studentId)
        {
            
            var parent = await _context.Parent.Include(s => s.Students).FirstOrDefaultAsync(s => s.Id == parentId);
            if (parent == null)
                return BadRequest();
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
                return BadRequest();
            parent.Students.Add(student);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(parent.Students == null ? "null" : "notnull");
            Console.ForegroundColor = ConsoleColor.White;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
