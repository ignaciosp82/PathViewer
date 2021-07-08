using System.Windows;

namespace PathViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new PathViewModel();
            InitializeComponent();
        }

        private void ZoomResetClick(object sender, RoutedEventArgs e) => (DataContext as PathViewModel).Zoom = 1;
    }
}
