using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WpMyApp.Models;

namespace WpMyApp.ViewModels
{
    public partial class ProjectsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Project> projects = new();

        public ProjectsViewModel()
        {
            Projects.Add(new Project { Name = "Проект 1", Description = "Описание проекта 1" });
            Projects.Add(new Project { Name = "Проект 2", Description = "Описание проекта 2" });
        }
    }
}
