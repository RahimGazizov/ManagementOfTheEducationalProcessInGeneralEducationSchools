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
builder.Services.AddScoped<ScheduleLogic>();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
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
async Task EnsureRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "јдмин", "”читель", "”ченик", "јдмнистраци€Ўколы" };

    foreach(var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));  
        }
    }
}
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/Authoriz/Index"; // адрес редиректа если пользователь не авторизован
    option.ExpireTimeSpan = TimeSpan.FromHours(1); // кука удалитьс€ после закрыти€ браузера
    option.SlidingExpiration = true; // не продливать автоматически
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
    option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    db.Database.ExecuteSqlRaw(@"
//        CREATE TABLE IF NOT EXISTS LessonSlots (
//            Id TEXT NOT NULL PRIMARY KEY,
//            LessonNumber INTEGER NOT NULL,
//            StartTime TEXT NOT NULL,
//            EndTime TEXT NOT NULL
//        );
//    ");
//}
using (var scope = app.Services.CreateScope())
{
    await EnsureRoles(scope.ServiceProvider);
}
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
// MVC стартова€
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authoriz}/{action=Index}/{id?}");



app.Run();
