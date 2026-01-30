namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class Class
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public int NumClass { get; set; }
        public char LetterClass { get; set; }
        public List<Students>? Students { get; set; }
    }
}
