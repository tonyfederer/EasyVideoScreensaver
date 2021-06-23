using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly string settingsFilename = ((App)Application.Current).settingsFilename;
        readonly MySettings Settings = ((App)Application.Current).settings;

        public MainWindow()
        {
            InitializeComponent();

            // Set player settings
            MediaPlayer.Source = new Uri(Settings.VideoFilename, UriKind.Absolute);
            switch (Settings.StretchMode)
            {
                case "Fill":
                    // Stretch to fit screen
                    MediaPlayer.Stretch = Stretch.Fill;
                    break;
                case "Center":
                    // Center in screen
                    MediaPlayer.Stretch = Stretch.None;
                    break;
                default:
                    // Fit to screen (maintain aspect ratio)
                    MediaPlayer.Stretch = Stretch.Uniform;
                    break;
            }

            MediaPlayer.Volume = Settings.Volume;
            MediaPlayer.IsMuted = Settings.Mute;
            MediaPlayer.Position = Settings.Position;

            // Detect when media ends
            MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
        }

        void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Loop video
            MediaPlayer.Position = TimeSpan.Zero;
        }

        void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Close screensaver when key is pressed
            e.Handled = true;

            SaveResumeIfChecked();

            Application.Current.Shutdown();
        }

        void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Close screensaver when mouse is clicked
            e.Handled = true;

            SaveResumeIfChecked();

            Application.Current.Shutdown();
        }

        void SaveResumeIfChecked()
        {
            if (!Settings.Resume)
                return;

            Settings.Position = MediaPlayer.Position;
            Settings.Save(settingsFilename);
        }
    }
}