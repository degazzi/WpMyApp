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

namespace WpMyApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ProjectService _projectService;

        [ObservableProperty]
        private string statusMessage = "Готово";

        [ObservableProperty]
        private ObservableCollection<ProjectTask> selectedProjectTasks = new();

        [ObservableProperty]
        private string selectedProjectName = "";

        [ObservableProperty]
        private ObservableCollection<Project> projects = new();

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
        private Models.OperationStatus status = new();

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

        [ObservableProperty]
        private double selectedProjectProgress = 0.0;

        [ObservableProperty]
        private string currentMonth = DateTime.Now.ToString("MMMM yyyy");

        [ObservableProperty]
        private DateTime today = DateTime.Today;

        public ObservableCollection<Executer> Executers { get; set; } = new();
        public ObservableCollection<Executer> SelectedExecuters { get; set; } = new();

        private Executer _newExecuter = new();
        public Executer NewExecuter
        {
            get => _newExecuter;
            set => SetProperty(ref _newExecuter, value);
        }


        private async Task LoadExecutersAsync()
        {
            var list = await _projectService.ExecuterService.GetAllAsync();
            Executers.Clear();
            foreach (var e in list)
                Executers.Add(e);
        }


        private async Task AddExecuterAsync()
        {
            if (string.IsNullOrWhiteSpace(NewExecuter.Name)) return;

            await _projectService.ExecuterService.CreateAsync(NewExecuter);

            Executers.Add(NewExecuter);

            NewExecuter = new Executer();
        }


        public MainViewModel()
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "ProjectManagementDB";

            _projectService = new ProjectService(connectionString, databaseName);

            AddExecuterCommand = new RelayCommand(async _ => await AddExecuterAsync());

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
            try
            {
                var projectList = await _projectService.GetProjectsAsync();
                Projects = new ObservableCollection<Project>(projectList);
                UpdateDashboardData();
                StatusMessage = $"Загружено {Projects.Count} проектов";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки проектов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            CompletedTasksCount = AllTasks.Count(t => t.Status == WpMyApp.Models.TaskStatus.Completed);
            OverdueTasksCount = AllTasks.Count(t => t.IsOverdue);
            UrgentProjectsCount = Projects.Count(p => p.IsUrgent);

            // Прогресс
            OverallProgress = TotalTasksCount > 0 ? (CompletedTasksCount / (double)TotalTasksCount) * 100 : 0;

            // Бюджет
            TotalBudget = Projects.Sum(p => p.Budget);
            SpentBudget = Projects.Sum(p => p.SpentBudget);

            // Прогресс выбранного проекта
            if (SelectedProject != null && !string.IsNullOrEmpty(SelectedProject.Id))
            {
                var projectTasks = AllTasks.Where(t => t.ProjectId == SelectedProject.Id).ToList();
                var completedProjectTasks = projectTasks.Count(t => t.Status == WpMyApp.Models.TaskStatus.Completed);
                SelectedProjectProgress = projectTasks.Count > 0 ? (completedProjectTasks / (double)projectTasks.Count) * 100 : 0;
            }

            // Последние проекты (4 самых новых)
            RecentProjects = new ObservableCollection<Project>(
                Projects.OrderByDescending(p => p.CreatedAt).Take(4)
            );

            // Ближайшие дедлайны задач
            UpcomingDeadlines = new ObservableCollection<ProjectTask>(
                AllTasks
                    .Where(t => !t.IsOverdue && t.Status != WpMyApp.Models.TaskStatus.Completed && t.DueDate > DateTime.Today)
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

            try
            {
                var success = await _projectService.SaveProjectAsync(SelectedProject);
                if (success)
                {
                    await LoadProjectsAsync();
                    SelectedProject = new Project();
                    StatusMessage = "Проект успешно сохранен";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка сохранения: {ex.Message}";
                MessageBox.Show($"Ошибка сохранения проекта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
                try
                {
                    var success = await _projectService.DeleteProjectAsync(SelectedProject.Id);
                    if (success)
                    {
                        await LoadProjectsAsync();
                        SelectedProject = new Project();
                        StatusMessage = "Проект успешно удален";
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка удаления: {ex.Message}";
                    MessageBox.Show($"Ошибка удаления проекта: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task DeleteProjectById(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return;

            var project = Projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
                return;

            var result = MessageBox.Show($"Удалить проект '{project.Name}'? Все задачи также будут удалены.",
                                       "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _projectService.DeleteProjectAsync(projectId);
                    if (success)
                    {
                        await LoadProjectsAsync();
                        StatusMessage = "Проект успешно удален";
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка удаления: {ex.Message}";
                    MessageBox.Show($"Ошибка удаления проекта: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private async Task SaveTaskAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedTask.Name) || string.IsNullOrEmpty(SelectedTask.ProjectId))
            {
                MessageBox.Show("Заполните название задачи и выберите проект", "Ошибка валидации",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var success = await _projectService.SaveTaskAsync(SelectedTask);
                if (success)
                {
                    await LoadProjectsAsync();
                    SelectedTask = new ProjectTask();
                    StatusMessage = "Задача успешно сохранена";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка сохранения: {ex.Message}";
                MessageBox.Show($"Ошибка сохранения задачи: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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

            var result = MessageBox.Show($"Удалить задачу '{SelectedTask.Name}'?",
                                       "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _projectService.DeleteTaskAsync(SelectedTask.Id);
                    if (success)
                    {
                        await LoadProjectsAsync();
                        SelectedTask = new ProjectTask();
                        StatusMessage = "Задача успешно удалена";
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Ошибка удаления: {ex.Message}";
                    MessageBox.Show($"Ошибка удаления задачи: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
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

            try
            {
                var searchResults = await _projectService.SearchProjectsAsync(SearchTerm);
                Projects = new ObservableCollection<Project>(searchResults);
                UpdateDashboardData();
                StatusMessage = $"Найдено {Projects.Count} проектов";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка поиска: {ex.Message}";
            }
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
            StatusMessage = "Создание нового проекта";
        }

        [RelayCommand]
        private void NewTask()
        {
            SelectedTask = new ProjectTask();
            if (SelectedProject != null && !string.IsNullOrEmpty(SelectedProject.Id))
            {
                SelectedTask.ProjectId = SelectedProject.Id;
            }
            StatusMessage = "Создание новой задачи";
        }

        [RelayCommand]
        private void NavigateToProgress()
        {
            StatusMessage = "Переход к прогрессу проектов";
        }

        [RelayCommand]
        private void NavigateToDeadlines()
        {
            StatusMessage = "Переход к дедлайнам";
        }

        [RelayCommand]
        private void NavigateToHome()
        {
            StatusMessage = "Главная страница";
        }

        [RelayCommand]
        private void NavigateToProjects()
        {
            StatusMessage = "Список проектов";
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            StatusMessage = "Настройки приложения";
        }

        [RelayCommand]
        private void OpenProject(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return;

            var proj = Projects.FirstOrDefault(x => x.Id == projectId);
            if (proj == null)
                return;

            SelectedProject = proj;
            SelectedProjectName = proj.Name;
            SelectedProjectTasks = new ObservableCollection<ProjectTask>(proj.Tasks);
            UpdateDashboardData();
            StatusMessage = $"Открыт проект: {proj.Name}";
        }

        [RelayCommand]
        private async Task AddProject()
        {
            SelectedProject = new Project();
            StatusMessage = "Создание нового проекта";
        }

        [RelayCommand]
        private async Task RefreshDashboard()
        {
            await LoadProjectsAsync();
            StatusMessage = "Дашборд обновлен";
        }

        [RelayCommand]
        private async Task CompleteTask(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
                return;

            var task = AllTasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null)
                return;

            task.Status = WpMyApp.Models.TaskStatus.Completed;
            task.CompletedDate = DateTime.UtcNow;

            try
            {
                await _projectService.SaveTaskAsync(task);
                await LoadProjectsAsync();
                StatusMessage = $"Задача '{task.Name}' выполнена";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка выполнения задачи: {ex.Message}";
                MessageBox.Show($"Ошибка выполнения задачи: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void EditTask(string taskId)
        {
            if (string.IsNullOrEmpty(taskId))
                return;

            SelectedTask = AllTasks.FirstOrDefault(t => t.Id == taskId);
            if (SelectedTask != null)
            {
                StatusMessage = $"Редактирование задачи: {SelectedTask.Name}";
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

        partial void OnSelectedProjectChanged(Project value)
        {
            if (value != null && !string.IsNullOrEmpty(value.Id))
            {
                SelectedProjectTasks = new ObservableCollection<ProjectTask>(value.Tasks);
                SelectedProjectName = value.Name;
                UpdateDashboardData();
            }
        }

        partial void OnSearchTermChanged(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                LoadProjectsCommand.ExecuteAsync(null);
            }
        }
    }
}