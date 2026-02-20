namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Lesson
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string HomeWork { get; set; }
        public string LessonTopic { get; set; }
        public string JournalId { get; set; }
        public Journal Journal { get; set; }
    }
}
