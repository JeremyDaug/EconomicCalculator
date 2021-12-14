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

namespace EditorInterface.Jobs
{
    /// <summary>
    /// Interaction logic for JobEditorWindow.xaml
    /// </summary>
    public partial class JobEditorWindow : Window
    {
        private Job job;
        private Manager manager;
        private JobViewModel viewModel;

        public JobEditorWindow()
        {
            InitializeComponent();

            manager = Manager.Instance;

            job = new Job();
            job.Id = manager.NewJobId;


            viewModel = new JobViewModel(job);
            DataContext = viewModel;
        }

        public JobEditorWindow(Job job)
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
