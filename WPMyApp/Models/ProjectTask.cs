using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using WpMyApp.Models;

namespace WpMyApp.Models
{
    public class ProjectTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public string ProjectId { get; set; }

        // Вычисляемые свойства для дашборда
        public bool IsOverdue => DueDate < DateTime.UtcNow && Status != TaskStatus.Completed;
        public bool IsDueSoon => (DueDate - DateTime.UtcNow).TotalDays <= 3 && !IsOverdue && Status != TaskStatus.Completed;
        public int DaysUntilDue => (int)(DueDate - DateTime.UtcNow).TotalDays;

        public string StatusDisplay => Status switch
        {
            TaskStatus.NotStarted => "Не начато",
            TaskStatus.InProgress => "В процессе",
            TaskStatus.Completed => "Выполнено",
            TaskStatus.OnHold => "На паузе",
            TaskStatus.Cancelled => "Отменено",
            _ => "Неизвестно"
        };

        public string PriorityDisplay => Priority switch
        {
            Priority.Low => "Низкий",
            Priority.Medium => "Средний",
            Priority.High => "Высокий",
            Priority.Critical => "Критический",
            _ => "Неизвестно"
        };

        public string DueDateDisplay => $"{DueDate:dd.MM.yyyy}";
        public string DaysLeftDisplay => IsOverdue ? "Просрочено" : $"{DaysUntilDue} дн.";
    }
}