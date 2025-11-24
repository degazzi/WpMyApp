// Services/DataService.cs
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WpMyApp.Models;
using WpMyApp.Data;

namespace WpMyApp.Services
{
    public class DataService
    {
        private readonly IMongoRepository<User> _userRepository;
        private readonly IMongoRepository<Product> _productRepository;
        private readonly OperationStatus _status;

        public DataService(string connectionString, string databaseName)
        {
            var databaseService = new DatabaseService(connectionString, databaseName);
            _userRepository = databaseService.GetRepository<User>("Users");
            _productRepository = databaseService.GetRepository<Product>("Products");
            _status = new OperationStatus();
        }

        public OperationStatus Status => _status;

        // User operations
        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                _status.SetStatus(StatusType.Loading, "Loading users...");
                var users = await _userRepository.GetAllAsync();
                _status.SetStatus(StatusType.Success, $"Loaded {users.Count} users");
                return users;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Failed to load users: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            try
            {
                _status.SetStatus(StatusType.Loading, "Loading user...");
                var user = await _userRepository.GetByIdAsync(id);
                _status.SetStatus(StatusType.Success, "User loaded successfully");
                return user;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Failed to load user: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveUserAsync(User user)
        {
            try
            {
                _status.SetStatus(StatusType.Saving, "Saving user...");

                if (string.IsNullOrEmpty(user.Id))
                {
                    await _userRepository.CreateAsync(user);
                    _status.SetStatus(StatusType.Success, "User created successfully");
                }
                else
                {
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user.Id, user);
                    _status.SetStatus(StatusType.Success, "User updated successfully");
                }
                return true;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Failed to save user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            try
            {
                _status.SetStatus(StatusType.Deleting, "Deleting user...");
                await _userRepository.DeleteAsync(id);
                _status.SetStatus(StatusType.Success, "User deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Failed to delete user: {ex.Message}");
                return false;
            }
        }

        // Product operations
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                _status.SetStatus(StatusType.Loading, "Loading products...");
                var products = await _productRepository.GetAllAsync();
                _status.SetStatus(StatusType.Success, $"Loaded {products.Count} products");
                return products;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Failed to load products: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<bool> SaveProductAsync(Product product)
        {
            try
            {
                _status.SetStatus(StatusType.Saving, "Saving product...");

                if (string.IsNullOrEmpty(product.Id))
                {
                    await _productRepository.CreateAsync(product);
                    _status.SetStatus(StatusType.Success, "Product created successfully");
                }
                else
                {
                    await _productRepository.UpdateAsync(product.Id, product);
                    _status.SetStatus(StatusType.Success, "Product updated successfully");
                }
                return true;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Failed to save product: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            try
            {
                _status.SetStatus(StatusType.Deleting, "Deleting product...");
                await _productRepository.DeleteAsync(id);
                _status.SetStatus(StatusType.Success, "Product deleted successfully");
                return true;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Failed to delete product: {ex.Message}");
                return false;
            }
        }

        // Search operations
        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                _status.SetStatus(StatusType.Loading, "Searching users...");
                var allUsers = await _userRepository.GetAllAsync();
                var filteredUsers = allUsers.FindAll(u =>
                    (u.FirstName != null && u.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (u.LastName != null && u.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Email != null && u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (u.Department != null && u.Department.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));

                _status.SetStatus(StatusType.Success, $"Found {filteredUsers.Count} users");
                return filteredUsers;
            }
            catch (Exception ex)
            {
                _status.SetStatus(StatusType.Error, $"Search failed: {ex.Message}");
                return new List<User>();
            }
        }
    }
}