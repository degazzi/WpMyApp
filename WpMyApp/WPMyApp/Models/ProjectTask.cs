using CommunityToolkit.Mvvm.ComponentModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WpMyApp.Models
{
    public class ProjectTask : ObservableObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string AssignedTo { get; set; }
        public List<string> ExecuterIds { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public string ProjectId { get; set; }

        public bool IsOverdue => Status != TaskStatus.Completed && DueDate < DateTime.UtcNow;

        public string DueDateString => DueDate.ToString("dd.MM.yyyy");

        public string DaysLeftDisplay
            => IsOverdue ? "Просрочено" : $"{(DueDate - DateTime.UtcNow).Days} дн.";

        public string DaysLeftColor
            => IsOverdue ? "#FF5555" : "#99FF99";

        public string DueDateDisplay => DueDate.ToString("dd.MM.yyyy");
    }
}
