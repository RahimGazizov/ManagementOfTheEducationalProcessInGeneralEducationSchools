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
        public AuthorizController(UserManager<Users> users)
        {
            _users = users;
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
            var result = await _users.CheckPasswordAsync(user, password);
            if (result)
                return RedirectToAction("Index", "Home");
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
