using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for BlackoutWindow.xaml
    /// </summary>
    public partial class VideoWindow : Window
    {
        private MySettings settings = ((App)Application.Current).settings;
        private string settingsFilename = ((App)Application.Current).settingsFilename;
        private MediaElement mediaElement;

        public VideoWindow(MediaElement media)
        {
            InitializeComponent();
            VisualBrush brush = new VisualBrush();
            mediaElement = media;
            brush.Visual = mediaElement;
            Display.Fill = brush;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Close screensaver when key is pressed
            e.Handled = true;
            CloseScreensaver();
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            //Close screensaver when mouse is moved
            e.Handled = true;
            CloseScreensaver();
        }

        private void CloseScreensaver()
        {
            //Save resume position
            if (settings.Resume)
            {
                settings.ResumePosition = mediaElement.Position.TotalSeconds;
                settings.Save(settingsFilename);
            }

            //Close screensaver
            Application.Current.Shutdown();
        }

    }
}
