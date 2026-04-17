namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Class
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int NumClass { get; set; }
        public string LetterClass { get; set; }
        public string AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public List<Students> Students { get; set; }
    }
}
