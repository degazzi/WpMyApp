    using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using System.Windows.Navigation;
using WpMyApp.Models;
using WpMyApp.Services;

namespace WpMyApp.ViewModels
{
    public partial class AddExecuterViewModel : ObservableObject
    {
        private readonly ExecuterService _service;

        [ObservableProperty] private string name;

        public AddExecuterViewModel(ExecuterService service)
        {
            _service = service;
        }

        [RelayCommand]
        private async Task Create()
        {
            if (string.IsNullOrWhiteSpace(Name)) return;

            await _service.CreateAsync(new Executer { Name = Name });

            Services.NavigationService.Instance.Back();
        }
    }
}
