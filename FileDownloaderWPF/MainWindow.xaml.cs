using System;
using System.Windows;
using FileDownloaderConsole;

namespace FileDownloaderWPF
{
    public partial class MainWindow : Window
    {
        string sourcePath;
        string destinationPath;
        int numberOfParallelism = 0;

        public MainWindow()
        {
            InitializeComponent();

            inputPathTextBox.TextChanged += InputPathTextBox_TextChanged;
            outputPathTextBox.TextChanged += OutputPathTextBox_TextChanged;
            
            outputPath.IsEnabled = false;
            outputPathTextBox.IsEnabled = false;

            downloadButton.IsEnabled = false;
        }

        private void InputPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();

            dialog.DefaultExt = "txt";
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                inputPathTextBox.Text = dialog.FileName;
                sourcePath = inputPathTextBox.Text;

                outputPath.IsEnabled = true;
                outputPathTextBox.IsEnabled = true;
            }
        }

        private void OutputPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                outputPathTextBox.Text = dialog.SelectedPath;
                destinationPath = outputPathTextBox.Text;
                downloadButton.IsEnabled = true;
            }
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DownloadWindow downloadWindow;

                if (numberOfParallelism != 0)
                {
                    downloadWindow = new DownloadWindow(sourcePath, destinationPath, numberOfParallelism);
                }
                else
                {
                    downloadWindow = new DownloadWindow(sourcePath, destinationPath);
                }
                    downloadWindow.Show();

                Close();
            }
            catch (Exception exception)
            {
               Log.WriteToLog(exception);
               MessageBox.Show("Ошибка!" + exception.Message);
            }
        }

        private void InputPathTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            sourcePath = inputPathTextBox.Text;
        }

        private void OutputPathTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            destinationPath = outputPathTextBox.Text;
        }

        private void degreeOfParallelism_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (degreeOfParallelism.Text != null)
                {
                    numberOfParallelism = Convert.ToInt32(degreeOfParallelism.Text);
                }
            }
            catch(Exception exception)
            {
                degreeOfParallelism.Text = "4";

                Log.WriteToLog(exception);

                MessageBox.Show(exception.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            inputPathTextBox.Text = "";
            outputPathTextBox.Text = "";
            degreeOfParallelism.Text = "4";
        }
    }
}
