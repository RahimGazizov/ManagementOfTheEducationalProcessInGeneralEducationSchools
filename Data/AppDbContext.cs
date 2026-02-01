using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InformationSystemOfASchoolIducationalPortal.Models;
namespace InformationSystemOfASchoolIducationalPortal.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Students> Students { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Students>()
                .HasOne(s => s.User)
                .WithOne() // один пользователь = один студент
                .HasForeignKey<Students>(s => s.UserId)
                .IsRequired();

            modelBuilder.Entity<Students>()
                .HasOne(s => s.Class)
                .WithMany() // если один класс может иметь много студентов
                .HasForeignKey(s => s.ClassId)
                .IsRequired();
        }
    }

}
