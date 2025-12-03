using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WpMyApp.Models;

namespace WpMyApp.ViewModels
{
    public partial class ExecutersViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Executer> executers = new();

        public ExecutersViewModel()
        {
            Executers.Add(new Executer { Name = "Иван" });
            Executers.Add(new Executer { Name = "Мария" });
        }
    }
}
