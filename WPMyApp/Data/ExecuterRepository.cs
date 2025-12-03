using MongoDB.Driver;
using WpMyApp.Models;

namespace WpMyApp.Data
{
    public class ExecuterRepository : MongoRepository<Executer>
    {
        public ExecuterRepository(IMongoDbContext context)
            : base(context.GetCollection<Executer>("Executers"))
        {
        }
    }
}
