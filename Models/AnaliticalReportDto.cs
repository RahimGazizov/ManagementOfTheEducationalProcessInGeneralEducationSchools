namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class AnaliticalReportDto
    {
        public string AcademicId { get; set; }
        public string TermId { get; set; }
        public string ClassId { get; set; }
        public string SubjectId { get; set; }

        public string ClassName { get; set; }
        public string SubjectName { get; set; }
        public List<DataStudents> DataStudents { get; set; } 
    }
}
