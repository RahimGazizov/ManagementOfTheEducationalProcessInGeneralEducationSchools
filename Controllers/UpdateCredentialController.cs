using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Authorize]
    public class UpdateCredentialController : Controller
    {

        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;
        public UpdateCredentialController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult CredentialsChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CredentialsChange([FromBody]UpdateCredentialsModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return BadRequest(new { message = "Пользователь не найден" });
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if(!result.Succeeded)
                return BadRequest(new { message = string.Join(", ", result.Errors.Select(s => s.Description))});
            user.UserName = model.UserName;
            user.IsCredentialsChanged = true;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(new { message = string.Join(", ", updateResult.Errors.Select(s => s.Description)) });
            return Ok(new { message = "Данные изменены" });
        }
    }
}
