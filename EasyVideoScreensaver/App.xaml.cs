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
        private HwndSource previewHwndSource;
        private MainWindow mainWindow;

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
                //If no parameters were specified, show setings
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
            foreach (Monitor m in Monitor.AllMonitors)
            {
                if (m.IsPrimary)
                {
                    //Show screensaver on primary monitor
                    mainWindow = new MainWindow();
                    mainWindow.WindowState = WindowState.Maximized;
                    mainWindow.Show();
                }
                else
                {
                    //Show black screen on all other monitors
                    BlackoutWindow window = new BlackoutWindow();
                    window.Top = m.Bounds.Top / (m.DpiY / 96);
                    window.Left = m.Bounds.Left / (m.DpiX / 96);
                    window.Height = m.Bounds.Height / (m.DpiY / 96);
                    window.Width = m.Bounds.Width / (m.DpiX / 96);
                    window.Show();
                }
            }
        }

        private void ShowPreview(IntPtr pPreviewHnd)
        {
            mainWindow = new MainWindow();

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
            previewHwndSource.RootVisual = mainWindow.grid1;
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

    }
}
