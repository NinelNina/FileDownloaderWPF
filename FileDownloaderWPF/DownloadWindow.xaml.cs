using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FileDownloaderConsole;

namespace FileDownloaderWPF
{
    public partial class DownloadWindow : Window
    {
        private int fileDownloadedCount = 0;
        private int fileUndownloadedCount = 0;
        private int fullDownloadProgress = 0;

        private double numberOfFiles;
        public InputData inputData;
        private List<FileData> items;

        void fullProgressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                (sender as BackgroundWorker).ReportProgress(fullDownloadProgress);
                System.Threading.Thread.Sleep(100);

            } while (fullDownloadProgress <= 100);
        }

        void fullProgressWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            fullProgress.Value = e.ProgressPercentage;
        }

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
            
            items = new List<FileData>();

            FileDownloader fileDownloader = new FileDownloader();

            fileDownloader.OnDownloaded += CountDownloadedFiles;
            fileDownloader.OnFailed += CountUndownloadedFiles;
            fileDownloader.OnFileProgress += CountReadBytes;

            BackgroundWorker fullProgressWorker = new BackgroundWorker();
            fullProgressWorker.WorkerReportsProgress = true;
            fullProgressWorker.DoWork += fullProgressWorker_DoWork;
            fullProgressWorker.ProgressChanged += fullProgressWorker_ProgressChanged;

            fullProgressWorker.RunWorkerAsync();


            int index = 1;

            try
            {
                foreach (string url in inputData.fileUrls)
                {

                    fileDownloader.AddFileToDownloadingQueue(Convert.ToString(index), url, inputData.PathToSave);

                    string fileName = FileExtension.GetFileName(url);

                    items.Add(new FileData() {FileID = Convert.ToString(index), FileName = fileName, Size = 0, Progress = 0 });
                    fileList.ItemsSource = items;

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

        public void CountDownloadedFiles(string fileId)
        {
            fileDownloadedCount++;

            ShowNumberOfDownloadedFiles();

            foreach (var item in items)
            {
                if (item.FileName.Contains(fileId))
                {
                    if (item.Size == 0)
                    {
                        item.Progress = 100;
                    }

                    return;
                }
            }
        }

        private void ShowNumberOfDownloadedFiles()
        {
            double downloadedFilesTemp = (Convert.ToDouble(fileDownloadedCount + fileUndownloadedCount) / Convert.ToDouble(numberOfFiles)) * 100;
            fullDownloadProgress = (int)Math.Round(downloadedFilesTemp);

            if (fullDownloadProgress == 100)
            {
                MessageBox.Show($"Загружено файлов: {fileDownloadedCount}. Загружено с ошибками: {fileUndownloadedCount}", "Загрузка выполнена", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void CountUndownloadedFiles(string fileId, Exception exception)
        {
            fileUndownloadedCount++;

            Log.WriteToLog(fileId, exception);

            ShowNumberOfDownloadedFiles();
        }

        public void CountReadBytes(string fileId, int totalValue, int downloadedBytes)
        {
            int progress = 1;

            if (totalValue != 0)
            {
                double downloadedBytesTemp = downloadedBytes / totalValue;
                progress = (int)Math.Round(downloadedBytesTemp * 100);
            }

            foreach (var item in items)
            {
                if (item.FileID.Contains(fileId))
                {
                    if (item.Size != 0)
                    {
                        item.Progress = progress;
                    }
                    else
                    {
                        item.Size = totalValue;
                        item.Progress = progress;
                    }

                    return;
                }
            }
        }
    }
}
