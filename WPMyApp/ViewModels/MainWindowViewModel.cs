using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using WpMyApp.Services;

namespace WpMyApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ProjectService _projectService;

        // Теперь это автогенерируемое свойство, без конфликтов
        [ObservableProperty]
        private ObservableObject currentViewModel;

        public MainWindowViewModel(ProjectService projectService)
        {
            _projectService = projectService;

            NavigationService.Instance.ViewModelChanged += OnViewModelChanged;

            // Стартовая страница
            var start = new DashboardViewModel();
            CurrentViewModel = start;
            NavigationService.Instance.Navigate(start);

            NavigationService.Instance.ViewModelChanged += vm =>
            {
                Debug.WriteLine($"VM changed to: {vm.GetType().Name}");
            };
        }

        private void OnViewModelChanged(ObservableObject vm)
        {
            CurrentViewModel = vm; // Toolkit сам вызовет OnPropertyChanged
        }

        // Команды
        [RelayCommand]
        private void GoHome() =>
            NavigationService.Instance.Navigate(new DashboardViewModel());

        [RelayCommand]
        private void GoProjects() =>
            NavigationService.Instance.Navigate(new ProjectsViewModel());

        [RelayCommand]
        private void GoExecuters() =>
            NavigationService.Instance.Navigate(new ExecutersViewModel());

        [RelayCommand]
        private void GoSettings() =>
            NavigationService.Instance.Navigate(new SettingsViewModel());

        [RelayCommand]
        private void GoDeadlines() =>
            NavigationService.Instance.Navigate(new DeadlinesViewModel());

        [RelayCommand]
        private void AddProject() =>
            NavigationService.Instance.Navigate(new AddProjectViewModel(_projectService));
    }
}
