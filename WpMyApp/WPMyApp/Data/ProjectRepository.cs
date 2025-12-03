using WpMyApp.Models;

namespace WpMyApp.Data
{
    public class ProjectRepository : MongoRepository<Project>, IMongoRepository<Project>
    {
        public ProjectRepository(IMongoDbContext context)
            : base(context.GetCollection<Project>("Projects"))
        {
        }
    }
}
