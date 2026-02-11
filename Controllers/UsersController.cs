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
                    UserId = user.Id,
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
            Console.WriteLine("Номера телефона" + createUser.PhoneNumber);
            var result = await _crudUser.AddUser(createUser);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message;
                var roles = await GetRoles();
                ViewBag.Roles = await GetSelectList(roles);
                ViewBag.TemproryPassword = createUser.Password;
                return View(createUser);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id, string returnUrl)
        {
            var result = await _crudUser.Delete(id);
            if (!result.Succeeded)
                return RedirectToAction("Index", new { error = result.Message });
            return Redirect(returnUrl);
        }
        public async Task<IActionResult> EditUser(string id, string returnUrl)
        {
            var user = await _users.FindByIdAsync(id);
            var roles = await _users.GetRolesAsync(user);
            var dto = new EditUserDTO
            {
                UserId = user.Id,
                FullName = user.FullName,
                Login = user.UserName,
                PhoneNumber = user.PhoneNumber,
                BirthDate = user.BirthDate,
                Role = roles.FirstOrDefault()
            };
            if (roles.Contains("Ученик"))
            {
                var student = await _context.Students.Include(c => c.Class)
                    .FirstOrDefaultAsync(u => u.UserId == user.Id);
                dto.StudentClassId = student.ClassId;
                dto.StudentClassNumber = student.Class?.NumClass ?? 0;
                dto.StudentClassLetter = student.Class?.LetterClass ?? "нет буквы";
            }
            ViewBag.Roles = await GetSelectList(await GetRoles());
            TempData["Url"] = returnUrl;
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserDTO dto,string returnUrl)
        {
            var result = await _crudUser.Edit(dto);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message;
                ViewBag.Roles = await GetSelectList(await GetRoles());
                ViewBag.Url = returnUrl;
                ViewData["ReturnUrl"] = returnUrl;
                return View(dto);
            }
            return Redirect(returnUrl);
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
        public async Task<IActionResult> ResetPassword(string login, string newPassword, string returnUrl)
        {
            var result = await _crudUser.ResetPassword(login, newPassword);
            if (!result.Succeeded)
            {
                TempData["Error"] = result.Message;
                return View();
            }
            return Redirect(returnUrl);
        }
        public async Task<JsonResult> GetLetterClass(int numClass)
        {
            var letters = await _context.Classes
        .Where(c => c.NumClass == numClass)
        .Select(c => new { c.Id, c.LetterClass })
        .OrderBy(c => c.LetterClass)
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
