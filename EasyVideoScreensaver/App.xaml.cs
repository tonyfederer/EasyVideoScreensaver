using System;
using System.Windows;
using System.Windows.Interop;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        HwndSource previewHwndSource;
        MainWindow mainWindow;

        public string settingsFilename;
        public MySettings settings;

        void ApplicationStartup(object sender, StartupEventArgs e)
        {
            settingsFilename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\VideoScreensaver.xml";
            settings = MySettings.Load(settingsFilename);

            string[] args = e.Args;

            if (e.Args.Length > 0 && e.Args[0].Contains(":"))
                args = e.Args[0].Split(':');

            if (args.Length == 0)
            {
                // If no parameters were specified, show setings
                ShowSettings();
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "/s":
                        // Show screensaver
                        ShowScreensaver();
                        return;
                    case "/p":
                        // Preview
                        if (e.Args.Length > 1)
                        {
                            var handle = Convert.ToInt32(e.Args[1]);
                            var hWnd = new IntPtr(handle);
                            ShowPreview(hWnd);
                        }
                        return;
                    case "/c":
                        // Settings
                        ShowSettings();
                        return;
                    default:
                        // Invalid parameter
                        MessageBox.Show("Invalid command line parameter: " + args[0], "Video Screensaver", MessageBoxButton.OK, MessageBoxImage.Error);
                        Current.Shutdown();
                        return;
                }
            }
        }

        void ShowScreensaver()
        {
            foreach (var m in Monitor.AllMonitors)
            {
                if (m.IsPrimary)
                {
                    // Show screensaver on primary monitor
                    mainWindow = new MainWindow
                    {
                        WindowState = WindowState.Maximized
                    };

                    mainWindow.Show();
                }
                else
                {
                    // Show black screen on all other monitors
                    var window = new BlackoutWindow
                    {
                        Top = m.Bounds.Top / (m.DpiY / 96),
                        Left = m.Bounds.Left / (m.DpiX / 96),
                        Height = m.Bounds.Height / (m.DpiY / 96),
                        Width = m.Bounds.Width / (m.DpiX / 96)
                    };

                    window.Show();
                }
            }
        }

        void ShowPreview(IntPtr pPreviewHnd)
        {
            mainWindow = new MainWindow();

            var lpRect = new NativeMethods.RECT();
            bool retVal = NativeMethods.GetClientRect(pPreviewHnd, out lpRect);

            var sourceParams = new HwndSourceParameters("sourceParams")
            {
                PositionX = 0,
                PositionY = 0,
                Height = lpRect.Height,
                Width = lpRect.Width,
                ParentWindow = pPreviewHnd,
                WindowStyle = (int)(NativeMethods.WindowStyles.WS_VISIBLE | NativeMethods.WindowStyles.WS_CHILD | NativeMethods.WindowStyles.WS_CLIPCHILDREN)
            };

            previewHwndSource = new HwndSource(sourceParams);
            previewHwndSource.Disposed += new EventHandler(PreviewHwndSource_Disposed);
            previewHwndSource.RootVisual = mainWindow.grid1;
        }

        void ShowSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        void PreviewHwndSource_Disposed(object sender, EventArgs e)
        {
            // Close window when preview window is disposed
            mainWindow.Close();
        }
    }
}