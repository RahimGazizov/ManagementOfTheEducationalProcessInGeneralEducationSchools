using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.Data;
using InformationSystemOfASchoolIducationalPortal.Models;
using InformationSystemOfASchoolIducationalPortal.BissnessLogicUser;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<CRUDUser>();
builder.Services.AddScoped<CRUDClass>();
builder.Services.AddScoped<CRUDSubject>();
builder.Services.AddScoped<TeachingAssigmentLogic>();
builder.Services.AddScoped<TeacherPerAccLogic>();
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6; // или сколько хочешь

})
    .AddDefaultTokenProviders()
.AddEntityFrameworkStores<AppDbContext>();
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/Authoriz/Index"; // адрес редиректа если пользователь не авторизован
    option.ExpireTimeSpan = TimeSpan.FromHours(1); // кука удалиться после закрытия браузера
    option.SlidingExpiration = true; // не продливать автоматически
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
    option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// MVC стартовая
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authoriz}/{action=Index}/{id?}");



app.Run();
