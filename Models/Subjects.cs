namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Subjects
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }

    }
}
