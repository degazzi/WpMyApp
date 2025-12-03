using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace WpMyApp.Services
{
    public class NavigationService
    {
        public static NavigationService Instance { get; } = new();

        public event Action<ObservableObject>? ViewModelChanged;

        private ObservableObject _currentViewModel;
        public ObservableObject CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                ViewModelChanged?.Invoke(_currentViewModel);
            }
        }

        private readonly Stack<ObservableObject> _history = new();

        public void Navigate(ObservableObject viewModel)
        {
            Debug.WriteLine($"Navigate to: {viewModel.GetType().Name}");

            if (_currentViewModel != null)
                _history.Push(_currentViewModel);

            CurrentViewModel = viewModel;

            Debug.WriteLine($"After navigate: CurrentViewModel = {CurrentViewModel.GetType().Name}");

            if (_currentViewModel != null)
                _history.Push(_currentViewModel);

            CurrentViewModel = viewModel;
        }

        public void Back()
        {
            if (_history.Count > 0)
                CurrentViewModel = _history.Pop();
        }
    }
}
