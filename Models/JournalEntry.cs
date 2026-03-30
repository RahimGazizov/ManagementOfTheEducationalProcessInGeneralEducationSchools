namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class JournalEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string StudentId { get; set; }
        public Students Student { get; set; }
        public string JournalId { get; set; }
        public Journal Journal { get; set; }

        public int? Grade { get; set; } // оценка
        public bool IsPresent { get; set; } = false; // посещаемость
    }
}
