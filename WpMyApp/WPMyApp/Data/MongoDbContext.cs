using MongoDB.Driver;
using System;

namespace WpMyApp.Data
{
    public class MongoDbContext : IMongoDbContext, IDisposable
    {
        private readonly IMongoDatabase _database;
        private readonly MongoClient _client;
        private bool _disposed;

        public MongoDbContext(string connectionString, string databaseName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentNullException(nameof(databaseName));

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
