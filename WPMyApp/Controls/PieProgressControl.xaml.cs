using System.Windows;
using System.Windows.Controls;

namespace WpMyApp.Controls
{
    public partial class PieProgressControl : UserControl
    {
        public PieProgressControl()
        {
            InitializeComponent();
        }

        public double Percentage
        {
            get => (double)GetValue(PercentageProperty);
            set => SetValue(PercentageProperty, value);
        }

        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register(nameof(Percentage), typeof(double), typeof(PieProgressControl), new PropertyMetadata(0.0));
    }
}
