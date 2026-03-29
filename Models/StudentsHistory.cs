namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class StudentsHistory
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StudentId { get; set; }
        public Students Student { get; set; }
        public string ClassId { get; set; }
        public Class Class { get; set; }
        public string AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public string TermId { get; set; }
        public Term Term { get; set; }
    }
}
