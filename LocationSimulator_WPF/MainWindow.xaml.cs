// MainWindow.xaml.cs

using System.Windows;
using System.Windows.Controls;

namespace LocationSimulator_WPF
{
    public partial class MainWindow : Window
    {
        private SimulatorController _controller;

        public MainWindow()
        {
            InitializeComponent();

            _controller = new SimulatorController();
            this.DataContext = _controller;
        }

        // --- לוגיקה לשליטה גלובלית (Start All / Stop All) ---
        private void StartAllButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartAll();
        }

        private void StopAllButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.StopAll();
        }

        // --- לוגיקה לשליטה בחיישן ספציפי ---
        private void SensorStart_Click(object sender, RoutedEventArgs e)
        {
            var sensor = (sender as Button)?.DataContext as INavigationSensor;
            sensor?.Start();
        }

        private void SensorStop_Click(object sender, RoutedEventArgs e)
        {
            var sensor = (sender as Button)?.DataContext as INavigationSensor;
            sensor?.Stop();
        }
    }
}