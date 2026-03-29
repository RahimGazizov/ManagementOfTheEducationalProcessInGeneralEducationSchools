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
        public DbSet<Subjects> Subjects { get; set; }
        public DbSet<Teachers> Teachers { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<Journal> Journal { get; set; }
        public DbSet<JournalEntry> JournalEntry { get; set; }
        public DbSet<TeacherAssigment> TeacherAssigments { get; set; }
        public DbSet<SchoolAdministrations> Administrations { get; set; }
        public DbSet<ScheduleLesson> Schedules {  get; set; }
        public DbSet<LessonSlot> LessonSlots { get; set; }
        public DbSet<AcademicYear> AcademicYear { get; set; }
        public DbSet<Term> Term { get; set; }
        public DbSet<StudentsHistory> StudentsHistory { get; set; }
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
                .WithMany(c => c.Students) // если один класс может иметь много студентов
                .HasForeignKey(s => s.ClassId)
                 .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Teachers>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Teachers>(s => s.UserId)
                .IsRequired();
            modelBuilder.Entity<Admins>()
                .HasOne(s => s.User)
                .WithOne()
                .HasForeignKey<Admins>(s => s.UserId)
                .IsRequired();
        }
    }

}
