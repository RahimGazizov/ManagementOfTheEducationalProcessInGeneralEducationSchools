using NuGet.DependencyResolver;

namespace InformationSystemOfASchoolIducationalPortal.Models
{

    public class Journal
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Date { get; set; }
        public string TeacherId { get; set; }
        public Teachers Teacher { get; set; }

        public string SubjectId { get; set; }
        public Subjects Subject { get; set; }

        public string ClassId { get; set; }
        public Class Class { get; set; }
        public string? HomeWork { get; set; }
        public string? LessonTopic { get; set; }
        public bool IsLocked { get; set; } = true;

        public string AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public string TermId { get; set; }
        public Term Term { get; set; }
        public List<JournalEntry> Entries { get; set; } = new(); // записи по ученикам
    }
}
