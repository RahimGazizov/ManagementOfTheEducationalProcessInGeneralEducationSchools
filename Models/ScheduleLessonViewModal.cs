namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class ScheduleLessonViewModal
    {
        public List<ScheduleLesson> ScheduleLessons { get; set; } = new();
        public ScheduleLesson FormSchedule { get; set; }
        public bool IsOpenModalAdd { get; set; } = false;
        public bool IsOpenModalEdit { get; set; } = false;
    }
}
