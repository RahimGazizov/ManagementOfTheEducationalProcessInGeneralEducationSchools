using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class SchoolAdministrationsController : Controller
    {
        private readonly AppDbContext _context;
        public SchoolAdministrationsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var administation = await _context.Administrations
                .Include(s => s.User)
                .ToListAsync();
            return View(administation);
        }
    }
}
