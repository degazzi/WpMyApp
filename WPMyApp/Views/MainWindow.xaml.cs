using WpMyApp.ViewModels;
using System.Windows;

namespace WpMyApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}