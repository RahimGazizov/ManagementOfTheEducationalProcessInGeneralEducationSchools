using Microsoft.AspNetCore.Mvc.Rendering;

namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class StudentDashboardViewModel
    {
        public Students Student { get; set; }

        public Term CurrentTerm { get; set; }
        public AcademicYear CurrentYear { get; set; }

        public double AverageGrade { get; set; }

        public List<Subjects> Subjects { get; set; }
        public List<ScheduleLesson> Schedule { get; set; }

        public List<SelectListItem> AcademicYears { get; set; }
        public List<SelectListItem> Terms { get; set; }
        public List<SelectListItem> Classes { get; set; }
        public Class CurrentClass { get; set; }
    }
}
