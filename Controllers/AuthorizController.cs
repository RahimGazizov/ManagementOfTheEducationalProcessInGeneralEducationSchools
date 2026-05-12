using InformationSystemOfASchoolIducationalPortal.Service;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class AuthorizController : Controller
    {
        private readonly AuthorizService _authorizService;
        private readonly UserManager<Users> _userManager;
        public AuthorizController(AuthorizService authorizService, UserManager<Users> userManager)
        {
            _authorizService = authorizService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string login, string password)
        {
            var user = await _userManager.FindByNameAsync(login.Trim());
            var result = await _authorizService.Authoriz(user, password);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                ViewBag.Login = login;
                ViewBag.Password = password;
                return View();
            }
            return RedirectToAction(result.ActionName, result.ControllerName);
        }
    }
}
