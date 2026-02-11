using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class AuthorizController : Controller
    {
        private readonly UserManager<Users> _users;
        private readonly SignInManager<Users> _signInManager;
        public AuthorizController(UserManager<Users> users, SignInManager<Users> signInManager)
        {
            _users = users;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string login, string password)
        {
            var user = await _users.FindByNameAsync(login);

            if (user == null)
            {
                TempData["Error"] = "Пользователь с таким логином не существует";
                ViewBag.Login = login;
                ViewBag.Password = password;
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (result.Succeeded)
            {
                if (await _users.IsInRoleAsync(user, "Админ"))
                    return RedirectToAction("Index", "AdminPersonalAccount");
                if (await _users.IsInRoleAsync(user, "Учитель"))
                    return RedirectToAction("Index", "Teachers");
                return RedirectToAction("Index", "Authoriz");
            }
            else
            {
                TempData["Error"] = "Не верный логин или пароль";
                ViewBag.Login = login;
                ViewBag.Password = password;
                return View();
            }
        }
    }
}
