// Services/DatabaseService.cs
using MongoDB.Driver;
using WpMyApp.Data;

namespace WpMyApp.Services
{
    public class DatabaseService
    {
        private readonly IMongoDatabase _database;

        public DatabaseService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoRepository<T> GetRepository<T>(string collectionName) where T : class
        {
            var collection = _database.GetCollection<T>(collectionName);
            return new MongoRepository<T>(collection); // ⚠️ Передаем коллекцию, а не service
        }
    }
}