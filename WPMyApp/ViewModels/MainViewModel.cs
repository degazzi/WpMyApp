// ViewModels/MainViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpMyApp.Data; // ⚠️ Добавьте using для репозитория
using WpMyApp.Services; // ⚠️ Добавьте using для DatabaseService
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace WpMyApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IMongoRepository<User> _userRepository;
        private readonly IMongoRepository<Product> _productRepository;

        [ObservableProperty]
        private ObservableCollection<User> users = new();

        [ObservableProperty]
        private User selectedUser = new();

        public MainViewModel()
        {
            // Настройка подключения
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "WpMyAppDatabase";

            var databaseService = new DatabaseService(connectionString, databaseName);
            _userRepository = databaseService.GetRepository<User>("Users");
            _productRepository = databaseService.GetRepository<Product>("Products");
        }

        [RelayCommand]
        private async Task LoadUsersAsync()
        {
            var userList = await _userRepository.GetAllAsync();
            users = new ObservableCollection<User>(userList);
        }
    }
}