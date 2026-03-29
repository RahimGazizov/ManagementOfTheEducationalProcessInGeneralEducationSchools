namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Term
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } // 1 четверть 
        public int QuarterNumber { get; set; }
        public DateTime DateStartTerm { get; set; }
        public DateTime DateEndTerm { get; set; }
        public string AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
    }
}
