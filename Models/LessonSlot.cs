namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class LessonSlot
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int LessonNumber { get; set; }   // 1, 2, 3...
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }
}
