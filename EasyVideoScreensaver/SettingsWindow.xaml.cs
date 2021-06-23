using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        readonly string SettingsFilename = ((App)Application.Current).settingsFilename;
        readonly MySettings Settings = ((App)Application.Current).settings;

        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = Settings;

            // Load settings
            VideoFilenameTextBox.Text = Settings.VideoFilename;
            StretchModeComboBox.ItemsSource = new List<string> { "Fit", "Fill", "Center" };
            StretchModeComboBox.SelectedValue = Settings.StretchMode;
            VolumeSlider.Value = Settings.Volume;
            MuteCheckBox.IsChecked = Settings.Mute;

            // Set initial focus
            VideoFilenameTextBox.Focus();
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate video file exists
            if (!string.IsNullOrEmpty(VideoFilenameTextBox.Text) && !File.Exists(VideoFilenameTextBox.Text))
            {
                MessageBox.Show("The specified file does not exist.", "File does not exist", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            // Save settings
            Settings.VideoFilename = VideoFilenameTextBox.Text;
            Settings.StretchMode = (string)StretchModeComboBox.SelectedValue;
            Settings.Volume = VolumeSlider.Value;
            Settings.Mute = MuteCheckBox.IsChecked.HasValue && MuteCheckBox.IsChecked.Value;
            Settings.Resume = ResumeCheckBox.IsChecked.HasValue && ResumeCheckBox.IsChecked.Value;
            Settings.Save(SettingsFilename);

            // Close window
            Close();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close window
            Close();
        }

        void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Show open dialog
            var dialog = new OpenFileDialog
            {
                Title = "Select video file"
            };

            if (string.IsNullOrEmpty(Settings.VideoFilename))
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            }
            else
            {
                dialog.FileName = Settings.VideoFilename;
                dialog.InitialDirectory = new FileInfo(Settings.VideoFilename).DirectoryName;
            }

            dialog.Filter = @"Video Files|*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2;*.mov;*.wmv;*.avi;*.mkv;*.mk3d;*.m2ts;*.m2t;*.mts;*.ts;*.tts|MP4 Video Files |*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2|QuickTime Movie Files|*.mov|Windows Video Files|*.wmv;*.avi|MKV Video Files|*.mkv|MK3D video file|*.mk3d|MPEG-2 TS Video Files|*.m2ts;*.m2t;*.mts;*.ts;*.tts;*.webm|All Files (*.*)|*.*";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog().HasValue && dialog.ShowDialog().Value)
                VideoFilenameTextBox.Text = dialog.FileName;
        }

        void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolumeValueLabel.Content = VolumeSlider.Value.ToString("P0");
        }

        void MuteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VolumeSlider.IsEnabled = false;
        }

        void MuteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VolumeSlider.IsEnabled = true;
        }
    }
}