using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using System.Diagnostics;
using Microsoft.Phone.Tasks;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Windows;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Xml.Serialization;


namespace Tonality.ViewModels
{
    public class SoundData : ViewModelBase, IEquatable<SoundData>
    {
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string Items { get; set; }
        public string Groups { get; set; }
        public string SavePath { get; set; }
        #region New code
        
        public bool IsDownloaded
        {
            get
            {
                return IsolatedStorageFile.GetUserStoreForApplication().FileExists(this.SavePath) && this.Status == DownloadStatus.Downloaded;
            }
        }

        private DownloadStatus status;
        public DownloadStatus Status
        {
            get
            {
                return this.status;
            }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.RaisePropertyChanged(() => this.Status);
                }
            }
        }

        private int downloadProgress;
        public int DownloadProgress
        {
            get
            {
                return this.downloadProgress;
            }
            set
            {
                if (this.downloadProgress != value)
                {
                    this.downloadProgress = value;
                    this.RaisePropertyChanged(() => this.DownloadProgress);
                }
            }
        }
        #endregion
        [XmlIgnore]
        public RelayCommand<string> SaveSoundAsRingtone { get; set; }

        public RelayCommand<string> SaveSoundAs { get; set; }

        private async void ExecuteSaveSoundAs(string soundPath)
        {

            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(SavePath);

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedSaveFile = file;
            savePicker.FileTypeChoices.Add("MP3", new List<string>() { ".mp3" });
            savePicker.ContinuationData.Add("SourceSound", SavePath);
            savePicker.SuggestedFileName = this.Title;
            savePicker.PickSaveFileAndContinue();
        }
       
        private void ExecuteSaveSoundAsRingtone(string soundPath)
        {
            if (IsDownloaded == false)
            {
                MessageBox.Show("Will not download until you short press atleast once to play sound");
                return;
            }
            App.Current.RootVisual.Dispatcher.BeginInvoke(() =>
            {

                SaveRingtoneTask task = new SaveRingtoneTask();
                task.Source = new Uri("isostore:/" + this.SavePath);
                task.DisplayName = this.Title;
                task.Show();
            }
               );
        }
          
            
        
               
        public SoundData()
        {
            SaveSoundAsRingtone = new RelayCommand<string>(ExecuteSaveSoundAsRingtone);
            this.Status = DownloadStatus.NotDownloaded;
            this.DownloadProgress = 0;
        }

        public override bool Equals(object obj)
        {
            SoundData other = obj as SoundData;

            if (obj == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public bool Equals(SoundData other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Title.Equals(other.Title);
        }

        public static bool operator ==(SoundData a, SoundData b)
        {
            if (object.ReferenceEquals(a, b)) return true;
            if (object.ReferenceEquals(a, null)) return false;
            if (object.ReferenceEquals(b, null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(SoundData a, SoundData b)
        {
            if (object.ReferenceEquals(a, b)) return false;
            if (object.ReferenceEquals(a, null)) return true;
            if (object.ReferenceEquals(b, null)) return true;

            return !a.Equals(b);
        }

    }
}
