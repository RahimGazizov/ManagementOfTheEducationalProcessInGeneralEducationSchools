using InformationSystemOfASchoolIducationalPortal.Service;
using System.IO;

namespace InformationSystemOfASchoolIducationalPortal.Middlewares
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        public MaintenanceMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, SystemStateService systemState)
        {
            if (systemState.IsMaintenanceMode && !context.User.IsInRole("Админ"))
            {
                if (context.Request.Method != "GET")
                {
                    context.Response.StatusCode = 503;
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync("Система на обслуживании");
                    return;
                }
            }
            await _next(context);
        }
    }
}
