using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.Data;
namespace InformationSystemOfASchoolIducationalPortal.Controllers
{

    [Authorize(Roles = "Админ")]
    public class AdminPersonalAccountController : Controller
    {
        private readonly SignInManager<Users> _signIn;
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userMan;
        public AdminPersonalAccountController(SignInManager<Users> signInManager,AppDbContext context,
            UserManager<Users> manager)
        {
            _signIn = signInManager;
            _context = context;
            _userMan = manager;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userMan.GetUserId(User);
            var admin = await _context.Admins.Include(u => u.User)
                .FirstOrDefaultAsync(id => id.UserId == userId);
            return View(admin);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Authoriz");
        }
    }
}
