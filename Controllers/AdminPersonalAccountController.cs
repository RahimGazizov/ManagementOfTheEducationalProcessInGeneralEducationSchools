using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
namespace InformationSystemOfASchoolIducationalPortal.Controllers
{

    [Authorize(Roles = "Админ")]
    public class AdminPersonalAccountController : Controller
    {
        private readonly SignInManager<Users> _signIn;
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userMan;
        private readonly ActionLogService _actionLogService;
        private readonly BackupsService _backupsService;
        private readonly SystemStateService _systemStateService;
        public AdminPersonalAccountController(SignInManager<Users> signInManager, AppDbContext context,
            UserManager<Users> manager, ActionLogService actionLogService, BackupsService backupsService, 
            SystemStateService systemStateService)
        {
            _signIn = signInManager;
            _context = context;
            _userMan = manager;
            _actionLogService = actionLogService;
            _backupsService = backupsService;
            _systemStateService = systemStateService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userMan.GetUserId(User);
            var admin = await _context.Admins.Include(u => u.User)
                .FirstOrDefaultAsync(id => id.UserId == userId);
            var folder = "Backups";
            if (Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            var files = Directory.GetFiles(folder)
                .Select(s => Path.GetFileName(s))
                .ToList();
            var modelView = new ModelViewAdmin
            {
                Admin = admin,
                ListBackups = files
            };
            return View(modelView);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBackup()
        {
            var result = await _backupsService.CreateBackup();
            if (!result.Success)
                TempData["Error"] = result.Message;

            else
                TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RestoreBackup(string fileName)
        {
            var result = await _backupsService.RestoreBackup(fileName);
            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult MaintenanceOn()
        {
            _systemStateService.IsMaintenanceMode = true;
            return Ok();
        }

        [HttpPost]
        public IActionResult MaintenanceOff()
        {
            _systemStateService.IsMaintenanceMode = false;
            return Ok();
        }
        public async Task<IActionResult> LogOut()
        {
            await _signIn.SignOutAsync();
            await _actionLogService.LogAsync(
                "Выход пользователя",
                "User",
                null,
                "Администратор вышел с системы"
                );
            return RedirectToAction("Index", "Authoriz");
        }
    }
}
