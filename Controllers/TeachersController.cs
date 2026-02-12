using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Админ")]
    public class TeachersController : Controller
    {
        private readonly AppDbContext _context;
        public TeachersController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var teachers = await _context.Teachers.Include(u => u.User).ToListAsync();
            return View(teachers);
        }
    }
}
