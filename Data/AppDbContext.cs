using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.Models;
namespace InformationSystemOfASchoolIducationalPortal.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
    
}
