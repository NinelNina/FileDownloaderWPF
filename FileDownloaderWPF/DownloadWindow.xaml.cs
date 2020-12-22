using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using FileDownloaderConsole;

namespace FileDownloaderWPF
{
    public partial class DownloadWindow : Window
    {
        private int fileDownloadedCount = 0;
        private int fileUndownloadedCount = 0;
        private int fullDownloadProgress = 0;
        private int numberOfParallelism;

        private double numberOfFiles;
        public InputData inputData;
        private List<FileData> items;

        public DownloadWindow(string sourcePath, string destinationPath, int degreeOfParallelism)
        {
            inputData = new InputData();

            numberOfParallelism = degreeOfParallelism;
            inputData.PathToOpen = sourcePath;
            inputData.PathToSave = destinationPath;

            InitializeComponent();

            BeginDownloading();
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
            numberOfFiles = inputData.NumberOfFiles;
            
            items = new List<FileData>();

            FileDownloader fileDownloader = new FileDownloader();

            if (numberOfParallelism != 0)
            {
                fileDownloader.SetDegreeOfParallelism(numberOfParallelism);
            }

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

            WorkData workData = new WorkData();

            foreach (var item in items)
            {
                if (item.FileID.Contains(fileId))
                {
                    workData.item = item;
                    workData.progress = progress;

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

        public class WorkData
        {
            public FileData item;
            public int progress;
            public int totalValue;
        }

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
    }
}
