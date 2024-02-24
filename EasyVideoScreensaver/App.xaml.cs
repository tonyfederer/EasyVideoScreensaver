using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private HwndSource previewHwndSource;
        private VideoWindow mainWindow;
        private MediaElement media;

        public MySettings settings;
        public string settingsFilename;

        void ApplicationStartup(object sender, StartupEventArgs e)
        {
            settingsFilename = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\VideoScreensaver.xml";

            settings = MySettings.Load(settingsFilename);

            string[] args = e.Args;

            if (e.Args.Length > 0 && e.Args[0].Contains(":"))
                args = e.Args[0].Split(':');

            if (args.Length == 0)
            {
                //If no parameters were specified, show settings
                ShowSettings();
            }
            else
            {
                switch(args[0].ToLower())
                {
                    case "/s":
                        //Show screensaver
                        ShowScreensaver();
                        return;
                    case "/p":
                        //Preview
                        if (e.Args.Length > 1)
                        {
                            int handle = Convert.ToInt32(e.Args[1]);
                            IntPtr hWnd = new IntPtr(handle);
                            ShowPreview(hWnd);
                        }
                        return;
                    case "/c":
                        //Settings
                        ShowSettings();
                        return;
                    default:
                        //Invalid parameter
                        MessageBox.Show("Invalid command line parameter: " + args[0], "Video Screensaver", MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                        return;
                }
            }
        }

        private void ShowScreensaver()
        {
            LoadVideo();

            foreach (Monitor m in Monitor.AllMonitors)
            {
                //Show video window on all screens
                VideoWindow window = new VideoWindow(media);
                window.Top = m.Bounds.Top / (m.DpiY / 96);
                window.Left = m.Bounds.Left / (m.DpiX / 96);
                window.Height = m.Bounds.Height / (m.DpiY / 96);
                window.Width = m.Bounds.Width / (m.DpiX / 96);
                window.Show();
            }
        }

        private void ShowPreview(IntPtr pPreviewHnd)
        {
            LoadVideo();

            mainWindow = new VideoWindow(media);

            NativeMethods.RECT lpRect = new NativeMethods.RECT();
            bool retVal = NativeMethods.GetClientRect(pPreviewHnd, out lpRect);

            HwndSourceParameters sourceParams = new HwndSourceParameters("sourceParams");

            sourceParams.PositionX = 0;
            sourceParams.PositionY = 0;
            sourceParams.Height = lpRect.Height;
            sourceParams.Width = lpRect.Width;
            sourceParams.ParentWindow = pPreviewHnd;
            sourceParams.WindowStyle = (int)(NativeMethods.WindowStyles.WS_VISIBLE | NativeMethods.WindowStyles.WS_CHILD | NativeMethods.WindowStyles.WS_CLIPCHILDREN);

            previewHwndSource = new HwndSource(sourceParams);
            previewHwndSource.Disposed += new EventHandler(previewHwndSource_Disposed);
            previewHwndSource.RootVisual = mainWindow.Display;
        }

        private void ShowSettings()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        void previewHwndSource_Disposed(object sender, EventArgs e)
        {
            //Close window when preview window is disposed
            mainWindow.Close();
        }

        private void LoadVideo()
        {
            media = new MediaElement();
            if (settings.VideoFilenames.Count() > 0)
            {
                Random random = new Random();
                string randomVideo = settings.VideoFilenames[random.Next(0, settings.VideoFilenames.Length)];
                media.Source = new Uri(randomVideo, UriKind.Absolute);
            }
            switch (settings.StretchMode)
            {
                case "Fill":
                    //Stretch to fit screen
                    media.Stretch = Stretch.Fill;
                    break;
                case "Center":
                    //Center in screen
                    media.Stretch = Stretch.None;
                    break;
                default:
                    //Fit to screen (maintain aspect ratio)
                    media.Stretch = Stretch.Uniform;
                    break;
            }
            media.Volume = settings.Volume;
            media.IsMuted = settings.Mute;
            if (settings.Resume)
            {
                media.Position = TimeSpan.FromSeconds(settings.ResumePosition);
            }

            //Detect when media ends
            media.MediaEnded += Media_MediaEnded;
        }

        private void Media_MediaEnded(object sender, RoutedEventArgs e)
        {
            //Loop video
            media.Position = TimeSpan.Zero;
        }

    }
}
