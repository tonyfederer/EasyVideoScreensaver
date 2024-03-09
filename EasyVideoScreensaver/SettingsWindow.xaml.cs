using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using Microsoft.Win32;

namespace EasyVideoScreensaver
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private MySettings settings = ((App)Application.Current).settings;
        private string settingsFilename = ((App)Application.Current).settingsFilename;

        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = settings;

            //Load settings
            VideoFilenameTextBox.Text = VideoFilenameText(settings.VideoFilenames);
            StretchModeComboBox.ItemsSource = new List<string> { "Fit", "Fill", "Center" };
            StretchModeComboBox.SelectedValue = settings.StretchMode;
            VolumeSlider.Value = settings.Volume;
            MuteCheckBox.IsChecked = settings.Mute;
            LoadResumeOption(settings, settings.VideoFilenames);

            //Set initial focus
            VideoFilenameTextBox.Focus();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //Save settings
            settings.StretchMode = (string)StretchModeComboBox.SelectedValue;
            settings.Volume = VolumeSlider.Value;
            settings.Mute = MuteCheckBox.IsChecked == true;
            settings.Resume = ResumeCheckBox.IsChecked == true;
            settings.Save(settingsFilename);

            //Close window
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //Close window
            this.Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            //Show open dialog
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Video File";
            dialog.Multiselect = true;
            if (settings.VideoFilenames.Count() == 0)
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            else
            {
                dialog.InitialDirectory = new System.IO.FileInfo(settings.VideoFilenames[0]).DirectoryName;
            }

            dialog.Filter = @"Video Files|*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2;*.mov;*.wmv;*.avi;*.mkv;*.mk3d;*.m2ts;*.m2t;*.mts;*.ts;*.tts|MP4 Video Files |*.mp4;*.m4v;*.mp4v;*.3gp;*.3gpp;*.3g2;*.3gp2|QuickTime Movie Files|*.mov|Windows Video Files|*.wmv;*.avi|MKV Video Files|*.mkv|MK3D video file|*.mk3d|MPEG-2 TS Video Files|*.m2ts;*.m2t;*.mts;*.ts;*.tts|All Files (*.*)|*.*";
            dialog.CheckFileExists = true;
            dialog.FileOk += OnFileOk;
            if (dialog.ShowDialog() == true)
            {
                VideoFilenameTextBox.Text = VideoFilenameText(dialog.FileNames);
                settings.VideoFilenames = dialog.FileNames;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VolumeValueLabel.Content = VolumeSlider.Value.ToString("P0");
        }

        private void MuteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            VolumeSlider.IsEnabled = false;
        }

        private void MuteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            VolumeSlider.IsEnabled = true;
        }

        private string VideoFilenameText(string[] fileNames)
        {
            switch (fileNames.Count())
            {
                case 0:
                    return "";
                case 1:
                    return fileNames[0];
                default:
                    return new System.IO.FileInfo(fileNames[0]).DirectoryName;
            }
        }

        private void LoadResumeOption(MySettings settings, string[] files)
        {
            if (files.Count() > 1)
            {
                ResumeCheckBox.IsEnabled = false;
                ResumeCheckBox.IsChecked = false;
            }
            else
            {
                ResumeCheckBox.IsEnabled = true;
                ResumeCheckBox.IsChecked = settings.Resume;
            }
        }
        private void OnFileOk(object sender, CancelEventArgs e)
        {
            LoadResumeOption(settings, ((OpenFileDialog)sender).FileNames);
        }
    }
}
