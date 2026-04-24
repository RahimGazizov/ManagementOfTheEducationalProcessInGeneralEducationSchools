namespace InformationSystemOfASchoolIducationalPortal.Models
{
    public class ActionLog
    {
        public int Id { get; set; }

        // Кто сделал действие
        public string? UserId { get; set; }

        // Логин или ФИО пользователя
        public string? UserName { get; set; }

        // Роль пользователя: Администратор, Учитель, Ученик, Родитель
        public string? Role { get; set; }

        // Что сделал пользователь
        public string Action { get; set; } = string.Empty;

        // Над какой сущностью было действие
        public string? EntityName { get; set; }

        // ID записи, над которой было действие
        public string? EntityId { get; set; }

        // Подробности действия
        public string? Details { get; set; }

        // Когда произошло действие
        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
