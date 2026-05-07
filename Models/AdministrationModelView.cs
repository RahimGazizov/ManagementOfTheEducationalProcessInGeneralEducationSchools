using Microsoft.AspNetCore.Mvc.Rendering;

namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class AdministrationModelView
    {
        public SchoolAdministrations Administration { get; set; }
        public List<SelectListItem> AcademicYears { get; set; }
    }
}
