using MongoDB.Driver;

namespace WpMyApp.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
