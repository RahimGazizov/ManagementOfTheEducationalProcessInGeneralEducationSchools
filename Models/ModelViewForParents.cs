using Microsoft.AspNetCore.Mvc.Rendering;

namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class ModelViewForParents
    {
        public Parents Parent {  get; set; }
        public List<SelectListItem> Childrens { get; set; }
        public List<StudentAvgScore> Students { get; set; }
        public Dictionary<string, double> AvgDynamicsInfo { get; set; }
        public bool IsJournalError { get; set; } = false;
        public bool IsRatingError { get; set; } = false;
        public bool IsAvgGrade { get; set; } = false;
        public bool IsSchedule { get; set; } = false;
    }
}
