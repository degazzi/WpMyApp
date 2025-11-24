using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpMyApp.Models;
using WpMyApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace WpMyApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DataService _dataService;

        [ObservableProperty]
        private ObservableCollection<User> users = new();

        [ObservableProperty]
        private ObservableCollection<Product> products = new();

        [ObservableProperty]
        private User selectedUser = new();

        [ObservableProperty]
        private Product selectedProduct = new();

        [ObservableProperty]
        private string searchTerm = "";

        [ObservableProperty]
        private OperationStatus status = new();

        [ObservableProperty]
        private bool isUserFormEnabled = true;

        [ObservableProperty]
        private int totalUsers = 0;

        [ObservableProperty]
        private int totalProducts = 0;

        public MainViewModel()
        {
            string connectionString = "mongodb://localhost:27017";
            string databaseName = "WpMyAppDatabase";

            _dataService = new DataService(connectionString, databaseName);

            // Подписка на изменения статуса
            _dataService.Status.PropertyChanged += (s, e) =>
            {
                Status.SetStatus(_dataService.Status.CurrentStatus, _dataService.Status.Message);
                IsUserFormEnabled = !_dataService.Status.IsBusy;
            };

            // Автоматическая загрузка данных при старте
            Task.Run(async () =>
            {
                await LoadUsersAsync();
                await LoadProductsAsync();
            });
        }

        [RelayCommand]
        private async Task LoadUsersAsync()
        {
            var userList = await _dataService.GetUsersAsync();
            Users = new ObservableCollection<User>(userList);
            TotalUsers = Users.Count;
        }

        [RelayCommand]
        private async Task LoadProductsAsync()
        {
            var productList = await _dataService.GetProductsAsync();
            Products = new ObservableCollection<Product>(productList); // ⚠️ Исправлено: Product (не Products)
            TotalProducts = Products.Count;
        }

        [RelayCommand]
        private async Task SaveUserAsync() // ⚠️ Исправлено: SaveUserAsync (не SaveliserAsync)
        {
            if (string.IsNullOrWhiteSpace(SelectedUser.FirstName) ||
                string.IsNullOrWhiteSpace(SelectedUser.LastName))
            {
                MessageBox.Show("Please fill first and last name", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var success = await _dataService.SaveUserAsync(SelectedUser);
            if (success)
            {
                await LoadUsersAsync();
                SelectedUser = new User();
            }
        }

        [RelayCommand]
        private async Task DeleteUserAsync()
        {
            if (SelectedUser == null || string.IsNullOrEmpty(SelectedUser.Id))
            {
                MessageBox.Show("Please select a user to delete", "Selection Required",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedUser.FullName}?",
                                       "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _dataService.DeleteUserAsync(SelectedUser.Id);
                if (success)
                {
                    await LoadUsersAsync();
                    SelectedUser = new User();
                }
            }
        }

        [RelayCommand]
        private async Task SearchUsersAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm)) // ⚠️ Исправлено: IsNullOrWhiteSpace (не IsNullOrWhitespace)
            {
                await LoadUsersAsync();
                return;
            }

            var searchResults = await _dataService.SearchUsersAsync(SearchTerm);
            Users = new ObservableCollection<User>(searchResults);
            TotalUsers = Users.Count;
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchTerm = "";
            LoadUsersCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private void NewUser()
        {
            SelectedUser = new User();
        }

        [RelayCommand]
        private async Task SaveProductAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedProduct.Name))
            {
                MessageBox.Show("Please enter product name", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var success = await _dataService.SaveProductAsync(SelectedProduct); // ⚠️ Исправлено: пробел после await
            if (success)
            {
                await LoadProductsAsync();
                SelectedProduct = new Product();
            }
        }

        [RelayCommand]
        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null || string.IsNullOrEmpty(SelectedProduct.Id))
            {
                MessageBox.Show("Please select a product to delete", "Selection Required",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedProduct.Name}?",
                                       "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question); // ⚠️ Добавлена ;

            if (result == MessageBoxResult.Yes)
            {
                var success = await _dataService.DeleteProductAsync(SelectedProduct.Id);
                if (success)
                {
                    await LoadProductsAsync();
                    SelectedProduct = new Product();
                }
            }
        }

        [RelayCommand]
        private void NewProduct()
        {
            SelectedProduct = new Product();
        }
    }
}