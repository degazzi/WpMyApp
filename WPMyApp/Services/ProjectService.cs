using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WpMyApp.Data;
using WpMyApp.Models;
using WpMyApp.Services;
using WpMyApp.Data;
using WpMyApp.Models;

namespace WpMyApp.Services
{
    public class ProjectService
    {
        private readonly IMongoRepository<Project> _projectRepository;
        private readonly IMongoRepository<ProjectTask> _taskRepository;
        private readonly OperationStatus _status;

        public ProjectService(string connectionString, string databaseName)
        {
            var databaseService = new DatabaseService(connectionString, databaseName);
            _projectRepository = databaseService.GetRepository<Project>("Projects");
            _taskRepository = databaseService.GetRepository<ProjectTask>("Tasks");
            _status = new OperationStatus();
        }

        public OperationStatus Status => _status;

        // Project operations
        public async Task<List<Project>> GetProjectsAsync()
        {
            try
            {
                _status.SetStatus(StatusType.Loading, "Загрузка проектов...");
                var projects = await _projectRepository.GetAllAsync();

                // Загружаем задачи для каждого проекта
                foreach (var project in projects)
                {
                    project.Tasks = await GetTasksByProjectAsync(project.Id);
                }

                _status.SetStatus(StatusType.Success, $"Загружено {projects.Count} проектов");
                return projects;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Ошибка загрузки проектов: {ex.Message}");
                return new List<Project>();
            }
        }

        public async Task<Project> GetProjectByIdAsync(string id)
        {
            try
            {
                _status.SetStatus(StatusType.Loading, "Загрузка проекта...");
                var project = await _projectRepository.GetByIdAsync(id);
                if (project != null)
                {
                    project.Tasks = await GetTasksByProjectAsync(id);
                }
                _status.SetStatus(StatusType.Success, "Проект загружен");
                return project;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Ошибка загрузки проекта: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveProjectAsync(Project project)
        {
            try
            {
                _status.SetStatus(StatusType.Saving, "Сохранение проекта...");

                if (string.IsNullOrEmpty(project.Id))
                {
                    await _projectRepository.CreateAsync(project);
                    _status.SetStatus(StatusType.Success, "Проект создан");
                }
                else
                {
                    project.UpdatedAt = DateTime.UtcNow;
                    await _projectRepository.UpdateAsync(project.Id, project);
                    _status.SetStatus(StatusType.Success, "Проект обновлен");
                }
                return true;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Ошибка сохранения проекта: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProjectAsync(string id)
        {
            try
            {
                _status.SetStatus(StatusType.Deleting, "Удаление проекта...");

                // Удаляем все задачи проекта
                var tasks = await GetTasksByProjectAsync(id);
                foreach (var task in tasks)
                {
                    await _taskRepository.DeleteAsync(task.Id);
                }

                await _projectRepository.DeleteAsync(id);
                _status.SetStatus(StatusType.Success, "Проект удален");
                return true;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Ошибка удаления проекта: {ex.Message}");
                return false;
            }
        }

        // Task operations
        public async Task<List<ProjectTask>> GetTasksByProjectAsync(string projectId)
        {
            try
            {
                var allTasks = await _taskRepository.GetAllAsync();
                return allTasks.Where(t => t.ProjectId == projectId).ToList();
            }
            catch (Exception)
            {
                return new List<ProjectTask>();
            }
        }

        public async Task<bool> SaveTaskAsync(ProjectTask task)
        {
            try
            {
                _status.SetStatus(StatusType.Saving, "Сохранение задачи...");

                if (string.IsNullOrEmpty(task.Id))
                {
                    await _taskRepository.CreateAsync(task);
                    _status.SetStatus(StatusType.Success, "Задача создана");
                }
                else
                {
                    task.UpdatedAt = DateTime.UtcNow;
                    await _taskRepository.UpdateAsync(task.Id, task);
                    _status.SetStatus(StatusType.Success, "Задача обновлена");
                }
                return true;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Ошибка сохранения задачи: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(string id)
        {
            try
            {
                _status.SetStatus(StatusType.Deleting, "Удаление задачи...");
                await _taskRepository.DeleteAsync(id);
                _status.SetStatus(StatusType.Success, "Задача удалена");
                return true;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Ошибка удаления задачи: {ex.Message}");
                return false;
            }
        }

        // Search and filter operations
        public async Task<List<Project>> SearchProjectsAsync(string searchTerm)
        {
            try
            {
                _status.SetStatus(StatusType.Loading, "Поиск проектов...");
                var allProjects = await GetProjectsAsync();
                var filteredProjects = allProjects.Where(p =>
                    p.Name?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    p.Client?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                    p.ProjectManager?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true).ToList();

                _status.SetStatus(StatusType.Success, $"Найдено {filteredProjects.Count} проектов");
                return filteredProjects;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Ошибка поиска: {ex.Message}");
                return new List<Project>();
            }
        }

        public async Task<List<ProjectTask>> GetOverdueTasksAsync()
        {
            try
            {
                var allTasks = await _taskRepository.GetAllAsync();
                return allTasks.Where(t => t.IsOverdue).ToList();
            }
            catch (Exception)
            {
                return new List<ProjectTask>();
            }
        }
    }
}