using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Xml.Linq;
using WpMyApp.Models;
using WpMyApp.Services;

namespace WpMyApp.ViewModels
{
    public partial class AddProjectViewModel : ObservableObject
    {
        private readonly ProjectService _projectService;

        [ObservableProperty] private string name;
        [ObservableProperty] private string description;

        public AddProjectViewModel(ProjectService service)
        {
            _projectService = service;
        }

        [RelayCommand]
        private void GoAddProject()
        {
            Services.NavigationService.Instance.Navigate(
                new AddProjectViewModel(_projectService)
            );
        }

        [RelayCommand]
        private async Task Create()
        {
            if (string.IsNullOrWhiteSpace(Name)) return;

            var project = new Project
            {
                Name = Name,
                Description = Description
            };

            await _projectService.SaveProjectAsync(project);

            Services.NavigationService.Instance.Back();
        }

        [RelayCommand]
        private void Back() => Services.NavigationService.Instance.Back();
    }
}
