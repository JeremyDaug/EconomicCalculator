using EconomicCalculator;
using EconomicCalculator.DTOs.Jobs;
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

namespace Editor.Jobs
{
    /// <summary>
    /// Interaction logic for JobEditorWindow.xaml
    /// </summary>
    public partial class JobEditorWindow : Window
    {
        private JobDTO job;
        private DTOManager manager;
        private JobViewModel viewModel;

        public JobEditorWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            job = new JobDTO();
            job.Id = manager.NewJobId;


            viewModel = new JobViewModel(job);
            DataContext = viewModel;
        }

        public JobEditorWindow(JobDTO job)
        {
            InitializeComponent();

            this.job = job;

            viewModel = new JobViewModel(job);
            DataContext = viewModel;
        }

        private void ShiftToJob(object sender, MouseButtonEventArgs e)
        {
            viewModel.ShiftProcessToJob();
        }

        private void RemoveFromJob(object sender, MouseButtonEventArgs e)
        {
            viewModel.RemoveProcessFromJob();
        }
    }
}
