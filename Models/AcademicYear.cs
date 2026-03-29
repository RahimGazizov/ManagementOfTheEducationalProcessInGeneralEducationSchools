namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class AcademicYear
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } // 2025-2026
        public DateTime StartDateYear { get; set; }
        public DateTime EndDateYear { get; set; }
        public List<Term> Terms { get; set; } = new();
    }
}
