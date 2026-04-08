namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class ParentsViewModel
    {
        public List<Parents> Parent {  get; set; }
        public Parents Form {  get; set; }
        public bool IsAdd { get; set; } = false;
        public bool IsEdit { get; set; } = false;
    }
}
