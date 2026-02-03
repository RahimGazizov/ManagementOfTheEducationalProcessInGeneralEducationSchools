using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using InformationSystemOfASchoolIducationalPortal.Data;
namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<Users> _users;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly CRUDUser _crudUser;
        private readonly AppDbContext _context;
        public UsersController(UserManager<Users> users, RoleManager<IdentityRole> roles,
            CRUDUser crudUser, AppDbContext context)
        {
            _users = users;
            _roles = roles;
            _crudUser = crudUser;
            _context = context;
        }

        public async Task<IActionResult> Index(string? error)
        {
            await EnsureRoles(_roles);
            TempData["Error"] = error;
            var users = await GetUsersList();
            var usersRole = new List<CreateUser>();
            foreach (var user in users)
            {
                var role = await _users.GetRolesAsync(user);
                usersRole.Add(new CreateUser
                {
                    FullName = user.FullName,
                    Login = user.UserName,
                    BirthDate = user.BirthDate,
                    Role = string.Join(", ", role)
                });
            }
            return View(usersRole);
        }
        public async Task<IActionResult> AddUsers()
        {
            var roles = await GetRoles();
            ViewBag.Roles = await GetSelectList(roles);
            Random random = new Random();
            ViewBag.TemproryPassword = "Temp" + random.Next(1000, 9999) + "!";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddUsers(CreateUser createUser)
        {
            var result = await _crudUser.AddUser(createUser);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message;
                var roles = await GetRoles();
                ViewBag.Roles = await GetSelectList(roles);
                return View(createUser);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string login)
        {
            var result = await _crudUser.Delete(login);
            if (!result.Succeeded)
                return RedirectToAction("Index", new { error = result.Message });

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditUser(string login)
        {
            var user = await _users.FindByNameAsync(login);
            var roles = await _users.GetRolesAsync(user);
            if (user == null)
                return RedirectToAction("Index", new { error = "Пользователь не найден" });
            var create = new CreateUser
            {
                FullName = user.FullName,
                Login = user.UserName,
                Role = string.Join(",", roles),
                BirthDate = user.BirthDate,
            };
            ViewBag.Roles = await GetSelectList(await GetRoles());
            return View(create);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(CreateUser createUser)
        {
            var result = await _crudUser.Edit(createUser);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message;
                ViewBag.Roles = await GetSelectList(await GetRoles());
                return View(createUser);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ResetPassword(string login)
        {
            var user = await _users.FindByNameAsync(login);
            Random random = new Random();
            string temproryPassword = "Temp" + Convert.ToString(random.Next(1000, 9999)) + "!";
            ViewBag.TemproryPas = temproryPassword;
            ViewBag.Login = user.UserName;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string login, string newPassword)
        {
            var result = await _crudUser.ResetPassword(login, newPassword);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message;
                return View();
            }
            return RedirectToAction("Index");
        }
        public async Task<JsonResult> GetLetterClass(int numClass)
        {
            var letters = await _context.Classes
        .Where(c => c.NumClass == numClass)
        .Select(c => new { c.Id, c.LetterClass })
        .ToListAsync();
            return Json(letters);

        }
        private async Task<List<SelectListItem>> GetSelectList<T>(IEnumerable<T> data)
        {
            var list = data.Select(item => new SelectListItem
            {
                Value = item?.ToString(),
                Text = item?.ToString()
            }).ToList();
            return list;
        }
        private async Task<List<IdentityRole>> GetRoles() => await _roles.Roles.ToListAsync();
        private async Task<List<Users>> GetUsersList() => await _users.Users.ToListAsync();
        private async Task EnsureRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Админ", "Учитель", "Ученик" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
