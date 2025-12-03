using System.Windows;
using WpMyApp.ViewModels;

namespace WpMyApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }
    }
}
