using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using Microsoft.AspNetCore.Mvc;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    [Route("system")]
    public class SystemController : Controller
    {
        private SystemStateService _systemStateService;
        public SystemController(SystemStateService systemStateService)
        {
            _systemStateService = systemStateService;
        }
        [HttpGet("status")]
        public IActionResult Status()
        {
            return Json(new
            {
                isMaintenance = _systemStateService.IsMaintenanceMode
            });
        }
    }
}
