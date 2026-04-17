using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Threading.Tasks;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Админ")]
    public class ParentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CRUDUser _crudUser;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ParentsController(AppDbContext context, CRUDUser crudUser, UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _crudUser = crudUser;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: ParentsController
        public async Task<IActionResult> Index(string? error)
        {
            TempData["Error"] = error;
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
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteChild(string parentId, string studentId)
        {
            var existParent = await _context.Parent.Include(s => s.Students).FirstOrDefaultAsync(s => s.Id == parentId);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            existParent.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}