using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class AdminPersonalAccountController : Controller
    {
        [Authorize(Roles = "Админ")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
