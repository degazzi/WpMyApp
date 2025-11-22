// Data/MongoRepository.cs
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WpMyApp.Data
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection; // ⚠️ Исправлено: IMongoCollection (без s)

        public MongoRepository(IMongoCollection<T> collection) // ⚠️ Изменен конструктор
        {
            _collection = collection;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id); // ⚠️ Исправлено: Eq (не Eg)
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T item)
        {
            await _collection.InsertOneAsync(item);
        }

        public async Task UpdateAsync(string id, T item)
        {
            var filter = Builders<T>.Filter.Eq("_id", id); // ⚠️ Исправлено: Eq (не Eg)
            await _collection.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteAsync(string id) // ⚠️ Добавьте этот метод
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}