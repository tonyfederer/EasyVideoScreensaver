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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MySettings settings = ((App)Application.Current).settings;

        public MainWindow()
        {
            InitializeComponent();

            //Set player settings
            MediaPlayer.Source = new Uri(settings.VideoFilename, UriKind.Absolute);
            switch (settings.StretchMode)
            {
                case "Fill":
                    //Stretch to fit screen
                    MediaPlayer.Stretch = Stretch.Fill;
                    break;
                case "Center":
                    //Center in screen
                    MediaPlayer.Stretch = Stretch.None;
                    break;
                default:
                    //Fit to screen (maintain aspect ratio)
                    MediaPlayer.Stretch = Stretch.Uniform;
                    break;
            }
            MediaPlayer.Volume = settings.Volume;
            MediaPlayer.IsMuted = settings.Mute;

            //Detect when media ends
            MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            //Loop video
            MediaPlayer.Position = TimeSpan.Zero;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //Close screensaver when key is pressed
            e.Handled = true;
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Close screensaver when mouse is clicked
            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}
