using Microsoft.AspNetCore.Identity;

namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime BirthDate {  get; set; }
        public bool IsCredentialsChanged { get; set; } = false;
    }
}
