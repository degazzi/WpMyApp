using WpMyApp.Models;

namespace WpMyApp.Data
{
    public class ProjectTaskRepository : MongoRepository<ProjectTask>, IMongoRepository<ProjectTask>
    {
        public ProjectTaskRepository(IMongoDbContext context)
            : base(context.GetCollection<ProjectTask>("ProjectTasks"))
        {
        }
    }
}
