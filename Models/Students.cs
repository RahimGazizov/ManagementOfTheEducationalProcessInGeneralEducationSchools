namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Students
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public Users User { get; set; } = null!;
        public string? ClassId { get; set; } = null!;
        public Class? Class { get; set; } = null!;
    }
}
