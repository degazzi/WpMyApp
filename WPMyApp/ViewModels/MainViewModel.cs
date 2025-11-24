using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpMyApp.Models;
using WpMyApp.Services;
using WpMyApp.Models;
using WpMyApp.Services;

namespace WpMyApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ProjectService _projectService;

        [ObservableProperty]
        private ObservableCollection<Project> _projects = new();

        [ObservableProperty]
        private ObservableCollection<ProjectTask> allTasks = new();

        [ObservableProperty]
        private ObservableCollection<Project> recentProjects = new();

        [ObservableProperty]
        private ObservableCollection<ProjectTask> upcomingDeadlines = new();

        [ObservableProperty]
        private ObservableCollection<ProjectTask> overdueTasks = new();

        [ObservableProperty]
        private Project selectedProject = new();

        [ObservableProperty]
        private ProjectTask selectedTask = new();

        [ObservableProperty]
        private string searchTerm = "";

        [ObservableProperty]
        private Models.OperationStatus _status = new();

        [ObservableProperty]
        private bool isFormEnabled = true;

        [ObservableProperty]
        private int totalProjects = 0;

        [ObservableProperty]
        private int totalTasksCount = 0;

        [ObservableProperty]
        private int overdueTasksCount = 0;

        [ObservableProperty]
        private int completedTasksCount = 0;

        [ObservableProperty]
        private double overallProgress = 0;

        [ObservableProperty]
        private decimal totalBudget = 0;

        [ObservableProperty]
        private decimal spentBudget = 0;

        [ObservableProperty]
        private int urgentProjectsCount = 0;

        // Статистика для дашборда
        [ObservableProperty]
        private string currentMonth = DateTime.Now.ToString("MMMM yyyy");

        public MainViewModel()
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "ProjectManagementDB";

            _projectService = new ProjectService(connectionString, databaseName);

            // Подписка на изменения статуса
            _projectService.Status.PropertyChanged += (s, e) =>
            {
                Status.SetStatus(_projectService.Status.CurrentStatus, _projectService.Status.Message);
                IsFormEnabled = !_projectService.Status.IsBusy;
            };

            // Автоматическая загрузка данных при старте
            Task.Run(async () =>
            {
                await LoadProjectsAsync();
                UpdateDashboardData();
            });
        }

        [RelayCommand]
        private async Task LoadProjectsAsync()
        {
            var projectList = await _projectService.GetProjectsAsync();
            Projects = new ObservableCollection<Project>(projectList);
            UpdateDashboardData();
        }

        [RelayCommand]
        private async Task LoadAllTasksAsync()
        {
            var tasks = new List<ProjectTask>();
            foreach (var project in Projects)
            {
                tasks.AddRange(project.Tasks);
            }
            AllTasks = new ObservableCollection<ProjectTask>(tasks);
            UpdateDashboardData();
        }

        private void UpdateDashboardData()
        {
            TotalProjects = Projects.Count;

            // Обновляем все задачи
            var allTasksList = new List<ProjectTask>();
            foreach (var project in Projects)
            {
                allTasksList.AddRange(project.Tasks);
            }
            AllTasks = new ObservableCollection<ProjectTask>(allTasksList);

            // Статистика
            TotalTasksCount = AllTasks.Count;
            CompletedTasksCount = AllTasks.Count(t => t.Status == Models.TaskStatus.Completed);
            OverdueTasksCount = AllTasks.Count(t => t.IsOverdue);
            UrgentProjectsCount = Projects.Count(p => p.IsUrgent);

            // Прогресс
            OverallProgress = TotalTasksCount > 0 ? (CompletedTasksCount / (double)TotalTasksCount) * 100 : 0;

            // Бюджет
            TotalBudget = Projects.Sum(p => p.Budget);
            SpentBudget = Projects.Sum(p => p.SpentBudget);

            // Последние проекты (4 самых новых)
            RecentProjects = new ObservableCollection<Project>(
                Projects.OrderByDescending(p => p.CreatedAt).Take(4)
            );

            // Ближайшие дедлайны задач
            UpcomingDeadlines = new ObservableCollection<ProjectTask>(
                AllTasks
                    .Where(t => !t.IsOverdue && t.Status != Models.TaskStatus.Completed)
                    .OrderBy(t => t.DueDate)
                    .Take(5)
            );

            // Просроченные задачи
            OverdueTasks = new ObservableCollection<ProjectTask>(
                AllTasks.Where(t => t.IsOverdue).Take(5)
            );
        }

        [RelayCommand]
        private async Task SaveProjectAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedProject.Name))
            {
                MessageBox.Show("Введите название проекта", "Ошибка валидации",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var success = await _projectService.SaveProjectAsync(SelectedProject);
            if (success)
            {
                await LoadProjectsAsync();
                SelectedProject = new Project();
            }
        }

        [RelayCommand]
        private async Task DeleteProjectAsync()
        {
            if (SelectedProject == null || string.IsNullOrEmpty(SelectedProject.Id))
            {
                MessageBox.Show("Выберите проект для удаления", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить проект '{SelectedProject.Name}'? Все задачи также будут удалены.",
                                       "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _projectService.DeleteProjectAsync(SelectedProject.Id);
                if (success)
                {
                    await LoadProjectsAsync();
                    SelectedProject = new Project();
                }
            }
        }

        [RelayCommand]
        private async Task SaveTaskAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedTask.Title) || string.IsNullOrEmpty(SelectedTask.ProjectId))
            {
                MessageBox.Show("Заполните название задачи и выберите проект", "Ошибка валидации",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var success = await _projectService.SaveTaskAsync(SelectedTask);
            if (success)
            {
                await LoadProjectsAsync();
                SelectedTask = new ProjectTask();
            }
        }

        [RelayCommand]
        private async Task DeleteTaskAsync()
        {
            if (SelectedTask == null || string.IsNullOrEmpty(SelectedTask.Id))
            {
                MessageBox.Show("Выберите задачу для удаления", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить задачу '{SelectedTask.Title}'?",
                                       "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _projectService.DeleteTaskAsync(SelectedTask.Id);
                if (success)
                {
                    await LoadProjectsAsync();
                    SelectedTask = new ProjectTask();
                }
            }
        }

        [RelayCommand]
        private async Task SearchProjectsAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                await LoadProjectsAsync();
                return;
            }

            var searchResults = await _projectService.SearchProjectsAsync(SearchTerm);
            Projects = new ObservableCollection<Project>(searchResults);
            UpdateDashboardData();
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchTerm = "";
            LoadProjectsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private void NewProject()
        {
            SelectedProject = new Project();
        }

        [RelayCommand]
        private void NewTask()
        {
            SelectedTask = new ProjectTask();
            if (SelectedProject != null && !string.IsNullOrEmpty(SelectedProject.Id))
            {
                SelectedTask.ProjectId = SelectedProject.Id;
            }
        }

        [RelayCommand]
        private void MarkTaskAsCompleted()
        {
            if (SelectedTask != null && !string.IsNullOrEmpty(SelectedTask.Id))
            {
                SelectedTask.Status = Models.TaskStatus.Completed;
                SelectedTask.CompletedDate = DateTime.UtcNow;
                SaveTaskCommand.ExecuteAsync(null);
            }
        }

        [RelayCommand]
        private void RefreshDashboard()
        {
            LoadProjectsCommand.ExecuteAsync(null);
        }
    }
}