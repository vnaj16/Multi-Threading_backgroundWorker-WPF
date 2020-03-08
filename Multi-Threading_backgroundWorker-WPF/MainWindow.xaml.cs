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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Multi_Threading_backgroundWorker_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		BackgroundWorker worker = new BackgroundWorker();

		public MainWindow()
        {
            InitializeComponent();
        }

		private void btnDoSynchronousCalculation_Click(object sender, RoutedEventArgs e)
		{
			int max = 5000;
			pbCalculationProgress.Value = 0;
			lbResults.Items.Clear();

			int result = 0;
			for (int i = 0; i < max; i++)
			{
				if (i % 42 == 0)
				{
					lbResults.Items.Add(i);
					result++;
				}
				System.Threading.Thread.Sleep(1);
				pbCalculationProgress.Value = Convert.ToInt32(((double)i / max) * 100);
			}
			MessageBox.Show("Numbers between 0 and 5000 divisible by 7: " + result);
		}

		private void btnDoAsynchronousCalculation_Click(object sender, RoutedEventArgs e)
		{
			pbCalculationProgress.Value = 0;
			lbResults.Items.Clear();


			worker.WorkerReportsProgress = true;
			worker.WorkerSupportsCancellation = true;
			worker.WorkerReportsProgress = true;
			worker.DoWork += worker_DoWork;
			worker.ProgressChanged += worker_ProgressChanged;
			worker.RunWorkerCompleted += worker_RunWorkerCompleted;
			worker.RunWorkerAsync(5000);
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			worker.CancelAsync();
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			int max = (int)e.Argument;
			int result = 0;
			for (int i = 0; i < max; i++)
			{
				if (worker.CancellationPending == true)
				{
					e.Cancel = true;
					return;
				}
				int progressPercentage = Convert.ToInt32(((double)i / max) * 100);
				if (i % 42 == 0)
				{
					result++;
					(sender as BackgroundWorker).ReportProgress(progressPercentage, i);
				}
				else
					(sender as BackgroundWorker).ReportProgress(progressPercentage);
				System.Threading.Thread.Sleep(1);

			}
			e.Result = result;
		}

		void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pbCalculationProgress.Value = e.ProgressPercentage;
			if (e.UserState != null)
				lbResults.Items.Add(e.UserState);
		}

		void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				MessageBox.Show("Evento cancelado por el usuario");
			}
			else
			{
				MessageBox.Show("Numbers between 0 and 5000 divisible by 7: " + e.Result);
			}

		}
	}
}
