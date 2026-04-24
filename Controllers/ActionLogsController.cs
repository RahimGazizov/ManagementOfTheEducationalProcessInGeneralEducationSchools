using InformationSystemOfASchoolIducationalPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace InformationSystemOfASchoolIducationalPortal.Controllers
{
    public class ActionLogsController : Controller
    {
        private readonly AppDbContext _context;
        public ActionLogsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var logs = await _context.ActionLog
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(logs);
        }
    }
}
