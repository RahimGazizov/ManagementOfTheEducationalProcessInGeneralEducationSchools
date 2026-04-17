using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class EditParentDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Login {  get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Role {  get; set; }
        public List<Students> Students { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
