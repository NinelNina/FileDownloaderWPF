using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderWPF
{
    public class FileData : INotifyPropertyChanged
    {
        string fileNameValue;
        int sizeValue;
        int progressValue;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string FileID { set; get; }
        public string FileName
        {
            set
            {
                if (value != fileNameValue)
                {
                    fileNameValue = value;
                    NotifyPropertyChanged();
                }
            }

            get
            {
                return fileNameValue;
            }
        }
        public int Size
        {
            set
            {
                if (value != sizeValue)
                {
                    sizeValue = value;
                    NotifyPropertyChanged();
                }
            }

            get
            {
                return sizeValue;
            }
        }
        public int Progress
        {
            set
            {
                if (value != progressValue)
                {
                    progressValue = value;
                    NotifyPropertyChanged();
                }
            }

            get
            {
                return progressValue;
            }
        }

    }
}
