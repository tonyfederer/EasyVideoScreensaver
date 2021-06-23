using System.Windows;
using System.Windows.Input;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for BlackoutWindow.xaml
    /// </summary>
    public partial class BlackoutWindow : Window
    {
        public BlackoutWindow()
        {
            InitializeComponent();
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Close screensaver when key is pressed
            e.Handled = true;
            Application.Current.Shutdown();
        }

        void Window_MouseDown(object sender, MouseEventArgs e)
        {
            // Close screensaver when mouse is moved
            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}