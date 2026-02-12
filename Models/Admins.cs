namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Admins
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
