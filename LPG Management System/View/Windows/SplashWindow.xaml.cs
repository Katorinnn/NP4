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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LPG_Management_System.View.Windows
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            // Event handlers marked explicitly non-nullable
            worker.DoWork += worker_DoWork!;
            worker.ProgressChanged += worker_ProgressChanged!;
            worker.RunWorkerAsync();
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker worker)
            {
                for (int i = 0; i <= 100; i++)
                {
                    worker.ReportProgress(i);
                    Thread.Sleep(80);
                }
            }
            else
            {
                throw new InvalidOperationException("Sender is not a BackgroundWorker.");
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;

            if (progressBar.Value == 100)
            {
                Login mainwindow = new Login();
                Close();
                mainwindow.ShowDialog();
            }
        }
    }
}
