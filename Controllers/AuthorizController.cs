using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
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
        private readonly AppDbContext _context;
        private readonly ActionLogService _actionLogService;
        public AuthorizController(UserManager<Users> users, SignInManager<Users> signInManager, AppDbContext context,
            ActionLogService actionLogService)
        {
            _users = users;
            _signInManager = signInManager;
            _context = context;
            _actionLogService = actionLogService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(string login, string password)
        {
            try
            {
                var user = await _users.FindByNameAsync(login.Trim());

                if (user == null)
                {
                    TempData["Error"] = "Пользователь с таким логином не существует";
                    ViewBag.Login = login;
                    ViewBag.Password = password;
                    return View();
                }
                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, false);
                if (result.Succeeded)
                {
                    await _actionLogService.LogAsync(
                        "Вход пользователя",
                        "User",
                        user.Id,
                        $"Пользователь вошел в систему."
                        );
                    if (await _users.IsInRoleAsync(user, "Админ"))
                        return RedirectToAction("Index", "AdminPersonalAccount");
                    if (await _users.IsInRoleAsync(user, "Учитель"))
                        return RedirectToAction("Index", "TeacherPerAcc");
                    if (await _users.IsInRoleAsync(user, "Ученик"))
                        return RedirectToAction("Index", "StudentPerAcc");
                    if (await _users.IsInRoleAsync(user, "Родитель"))
                        return RedirectToAction("Index", "ParentPerAccount");
                    if (await _users.IsInRoleAsync(user, "АдминистрацияШколы"))
                        return RedirectToAction("Index", "AdministrationSchoolPerAcc");
                    return RedirectToAction("Index", "Authoriz");
                }
                else
                {
                    TempData["Error"] = "Не верный логин или пароль";
                    ViewBag.Login = login;
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }
    }
}
