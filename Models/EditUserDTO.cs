namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class EditUserDTO
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string Role { get; set; }

        // Для ученика
        public string? StudentClassId { get; set; }
        public int? StudentClassNumber { get; set; }
        public string? StudentClassLetter { get; set; }

        // Для учителя
        public List<string> TeacherSubjects { get; set; } = new();
        public List<string> TeacherClassIds { get; set; } = new();

        // Для родителя
        public string? Email { get; set; }
    }
}
