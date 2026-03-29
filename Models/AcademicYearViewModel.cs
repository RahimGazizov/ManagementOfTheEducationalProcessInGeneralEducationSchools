namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class AcademicYearViewModel
    {
        public List<AcademicYear> AcademicYears { get; set; } = new List<AcademicYear>();
        public AcademicYear From { get; set; }
        public bool IsAdd { get; set; } = false;
        public bool IsEdit { get; set; } = false;
    }
}
