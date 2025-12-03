using System.Windows;
using WpMyApp.Data;
using WpMyApp.Services;
using WpMyApp.Views;

namespace WpMyApp
{
    public partial class App : Application
    {
        public static ProjectService ProjectService { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // === СНАЧАЛА СОЗДАЁМ ЗАВИСИМОСТИ ===

            var context = new MongoDbContext("mongodb://localhost:27017", "WpMyAppDatabase");

            var projectRepo = new ProjectRepository(context);
            var taskRepo = new ProjectTaskRepository(context);
            var executerRepo = new ExecuterRepository(context);

            var executerService = new ExecuterService(executerRepo);
            var statusService = new StatusService();

            ProjectService = new ProjectService(projectRepo, taskRepo, executerService, statusService);

            // === ТОЛЬКО ПОСЛЕ ЭТОГО СОЗДАЁМ ГЛАВНОЕ ОКНО ===
            var window = new MainWindow();
            window.Show();
        }
    }
}
