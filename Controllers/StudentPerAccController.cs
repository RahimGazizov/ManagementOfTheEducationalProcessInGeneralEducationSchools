using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize(Roles = "Ученик")]
    public class StudentPerAccController : Controller
    {
        private readonly UserManager<Users> _userMan;
        private readonly SignInManager<Users> _signIn;
        private readonly AppDbContext _context;
        public StudentPerAccController(UserManager<Users> userMan, SignInManager<Users> signIn
            , AppDbContext context)
        {
            _userMan = userMan;
            _signIn = signIn;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userMan.GetUserId(User);
            var student = await _context.Students.Include(u => u.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
            return View(student);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index","Authoriz");
        }
    }
}
