using System.Windows;
using WpMyApp.Data;
using WpMyApp.Models;
using WpMyApp.Services;
using WpMyApp.ViewModels;
using WpMyApp.Views;

namespace WpMyApp
{
    public partial class App : Application
    {
    

    protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mongoConnection = "mongodb://localhost:27017";
            var mongoDbName = "WpMyAppDb";

            var context = new MongoDbContext(mongoConnection, mongoDbName);

            // передаём в репозитории и сервисы
            var executerRepo = new ExecuterRepository(context);
            var projectRepo = new ProjectRepository(context); // если ProjectRepository принимает IMongoCollection
                                                                                                 // или, если ProjectRepository есть конструктор (IMongoDbContext) — используем аналогично ExecuterRepository

            var executerService = new ExecuterService(executerRepo);
            var statusService = new StatusService();

            var projectTaskRepo = new ProjectTaskRepository(context); // пример, если такой конструктор есть
            var projectService = new ProjectService(projectRepo, projectTaskRepo, executerService, statusService);

            var mainVm = new MainViewModel(/*projectService*//* и другие зависимости */);
            var mainWindow = new MainWindow { DataContext = mainVm };
            mainWindow.Show();
        }


    }
}
