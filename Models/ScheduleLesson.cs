using System.ComponentModel.DataAnnotations.Schema;

namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class ScheduleLesson
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TeacherAssigmentId { get; set; }
        [ForeignKey("TeacherAssigmentId")]
        public TeacherAssigment Assigment { get; set; }
        public string AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
        public string DayOfWeek { get; set; }
        public string? LessonSlotId { get; set; }
        public LessonSlot? LessonSlot { get; set; }
        public string Room { get; set; }
    }
}
