using System.Security.Claims; // чтобы получить данные текущего авторизованного пользователя.
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore; // это информация о текущем пользователе и текущем запросе.

namespace InformationSystemOfASchoolIducationalPortal.Service
{

    public class ActionLogService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor; // Это штука, которая позволяет сервису узнать текущего пользователя.
        public ActionLogService(AppDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }

        public async Task LogAsync(
            string action,
            string? entityName = null,
            string? entityId = null,
            string? details = null)
        {
            var httpContext = _accessor.HttpContext;

            var userId = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier); // текущего пользователя
            var userName = httpContext?.User.Identity?.Name; // получаем логин либо почту
            var role = httpContext?.User.FindFirstValue(ClaimTypes.Role); // получаем айди пользвоателя
            
            var log = new ActionLog
            {
                UserId = userId,
                UserName = userName ?? "Неизвестный пользователь",
                Role = role ?? "Не указана",
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                CreatedAt = DateTime.Now
            };

            _context.ActionLog.Add(log);

            await _context.SaveChangesAsync();
        }
    }
}
