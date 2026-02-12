using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Учитель")]
    public class TeacherPerAccController : Controller
    {
        private readonly UserManager<Users> _userMan;
        private readonly SignInManager<Users> _signIn;
        private readonly AppDbContext _context;
        public TeacherPerAccController(UserManager<Users> userMan, AppDbContext context
            , SignInManager<Users> signIn)
        {
            _userMan = userMan;
            _context = context;
            _signIn = signIn;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userMan.GetUserId(User);
            var teacher = await _context.Teachers.Include(u => u.User)
                .FirstOrDefaultAsync(id => id.UserId == userId);
            return View(teacher);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Authoriz");
        }
    }
}
