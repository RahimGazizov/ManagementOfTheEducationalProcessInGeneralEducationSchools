namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class LessonSlotViewModal
    {
        public List<LessonSlot> LessonSlots { get; set; }
        public LessonSlot Form { get; set; }
        public bool IsOpenModalAdd { get; set; } = false;
        public bool IsOpenModalEdit { get; set; } = false;
    }
}
