namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Students
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public Users User { get; set; }
        public string ClassId { get; set; } 
        public Class Class { get; set; }
    }
}
