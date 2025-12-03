using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpMyApp.Services;

namespace WpMyApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            NavigationService.Instance.ViewModelChanged += vm =>
            {
                CurrentViewModel = vm;
            };

            // Стартовая страница
            NavigationService.Instance.Navigate(new DashboardViewModel());
        }

        [ObservableProperty]
        private ObservableObject currentViewModel;

        [RelayCommand]
        private void GoHome() =>
            NavigationService.Instance.Navigate(new DashboardViewModel());

        [RelayCommand]
        private void GoProjects() =>
            NavigationService.Instance.Navigate(new ProjectsViewModel());

        [RelayCommand]
        private void GoDeadlines() =>
            NavigationService.Instance.Navigate(new DeadlinesViewModel());

        [RelayCommand]
        private void GoSettings() =>
            NavigationService.Instance.Navigate(new SettingsViewModel());

        [RelayCommand]
        private void AddProject() =>
            NavigationService.Instance.Navigate(new AddProjectViewModel(null));
    }
}
