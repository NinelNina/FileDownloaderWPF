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
using FileDownloaderConsole;

namespace FileDownloaderWPF
{
    /// <summary>
    /// Логика взаимодействия для DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        //public string sourcePath;
        //public string destinationPath;

        private int fileDownloadedCount = 0;
        private int fileUndownloadedCount = 0;
        private double downloadedFiles;
        private double numberOfFiles;
        public InputData inputData;
        public DownloadWindow(string sourcePath, string destinationPath)
        {
            inputData = new InputData();

            inputData.PathToOpen = sourcePath;
            inputData.PathToSave = destinationPath;

            InitializeComponent();

            BeginDownloading();
        }

        private void BeginDownloading()
        {
            inputData.Input();
            numberOfFiles = inputData.numberOfFiles;

            FileDownloader fileDownloader = new FileDownloader();

            CountDownloadingFiles downloadingFiles = new CountDownloadingFiles();

            fileDownloader.OnDownloaded += CountDownloadedFiles;
            fileDownloader.OnFailed += CountUndownloadedFiles;

            int index = 1;

            try
            {
                foreach (string url in inputData.fileUrls)
                {

                    fileDownloader.AddFileToDownloadingQueue(Convert.ToString(index), url, inputData.PathToSave);
                    index++;
                }
            }
            catch (Exception exception)
            {
                Log.WriteToLog(exception);
                MessageBox.Show("Ошибка!" + exception.Message);

                return;
            }
        }

        public void CountDownloadedFiles(string message)
        {
            fileDownloadedCount++;

            downloadedFiles = (Convert.ToDouble(fileDownloadedCount) / Convert.ToDouble(numberOfFiles)) * 100;
            downloadedFiles = (int)Math.Round(downloadedFiles);

            MessageBox.Show($"Загрузка файла <<{message}>> {downloadedFiles}%");

            ShowNumberOfDownloadedFiles();
        }

        private void ShowNumberOfDownloadedFiles()
        {
            if (fileDownloadedCount + fileUndownloadedCount == numberOfFiles)
            {
                MessageBox.Show($"Загружено файлов: {fileDownloadedCount}. Загружено с ошибками: {fileUndownloadedCount}");
            }
        }

        public void CountUndownloadedFiles(string message, Exception exception)
        {
            fileUndownloadedCount++;

            Log.WriteToLog(message, exception);

            ShowNumberOfDownloadedFiles();
        }
    }
}
