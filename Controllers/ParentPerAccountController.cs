using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class ParentPerAccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;
        public ParentPerAccountController(UserManager<Users> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "Родитель")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var parent = await _context.Parent.Include(u => u.User).FirstOrDefaultAsync(u => u.UserId == userId);
            return View(parent);
        }
    }
}
