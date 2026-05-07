namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Parents
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public Users User { get; set; }
        public List<Students> Students { get; set; } = new();
        public string? Email { get; set; }

    }
}
