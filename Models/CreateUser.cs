namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class CreateUser
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string? ClassId { get; set; }
    }
}
