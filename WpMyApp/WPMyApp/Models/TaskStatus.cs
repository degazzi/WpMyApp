namespace WpMyApp.Models
{
    public enum TaskStatus
    {
        NotStarted,     // Не начато
        InProgress,     // В процессе
        Completed,      // Выполнено
        OnHold,         // На паузе
        Cancelled       // Отменено
    }
    public enum Priority
    {
        Low,           // Низкий
        Medium,        // Средний
        High,          // Высокий
        Critical       // Критический
    }
}