namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class UpdateCredentialsModel
    {
        public string UserName { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
