using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using WpMyApp.Models;

namespace WpMyApp.Models
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Client { get; set; }
        public string ProjectManager { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal SpentBudget { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonIgnore]
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        // Вычисляемые свойства для дашборда
        public int TotalTasks => Tasks.Count;
        public int CompletedTasks => Tasks.Count(t => t.Status == TaskStatus.Completed);
        public int PendingTasks => Tasks.Count(t => t.Status != TaskStatus.Completed);
        public int OverdueTasks => Tasks.Count(t => t.IsOverdue);
        public decimal BudgetUsage => Budget > 0 ? (SpentBudget / Budget) * 100 : 0;
        public double Progress => TotalTasks > 0 ? (CompletedTasks / (double)TotalTasks) * 100 : 0;
        public bool IsOverdue => EndDate < DateTime.UtcNow && Progress < 100;
        public int DaysUntilDeadline => (int)(EndDate - DateTime.UtcNow).TotalDays;
        public bool IsUrgent => DaysUntilDeadline <= 7 && DaysUntilDeadline > 0;

        public string StatusDisplay
        {
            get
            {
                if (Progress >= 100) return "Завершен";
                if (IsOverdue) return "Просрочен";
                if (IsUrgent) return "Срочный";
                if (Progress > 0) return "В работе";
                return "Планирование";
            }
        }

        public string ProgressDisplay => $"{Progress:F1}%";
        public string BudgetDisplay => $"{SpentBudget:C2} / {Budget:C2}";
        public string DeadlineDisplay => $"{EndDate:dd.MM.yyyy}";
        public string DaysLeftDisplay => DaysUntilDeadline > 0 ? $"{DaysUntilDeadline} дн." : "Просрочен";
    }
}