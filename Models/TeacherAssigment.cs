using System.ComponentModel.DataAnnotations.Schema;

namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class TeacherAssigment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeacherId { get; set; }
        public Teachers Teacher { get; set; }

        public string SubjectId { get; set; }
        public Subjects Subject { get; set; }

        public string ClassId { get; set; }
        public Class Class { get; set; }
        public string AcademicId { get; set; }
        [ForeignKey("AcademicId")]
        public AcademicYear AcademicYear { get; set; }
    }
}
