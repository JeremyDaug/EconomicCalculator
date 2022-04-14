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
    /// Interaction logic for JobsListWindow.xaml
    /// </summary>
    public partial class JobsListWindow : Window
    {
        public IList<IJobDTO> jobs;

        private DTOManager manager;

        public JobsListWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            jobs = manager.Jobs.Values.ToList();

            JobsGrid.ItemsSource = jobs;
        }

        private void JobsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditJob(sender, e);
        }

        private void NewJob(object sender, RoutedEventArgs e)
        {
            var job = new JobDTO
            {
                Id = manager.NewJobId,
                Name = ""
            };

            Window win = new JobEditorWindow(job);

            win.ShowDialog();
            JobsGrid.ItemsSource = manager.Jobs.Values.ToList();
            JobsGrid.Items.Refresh();
        }

        private void CopyJob(object sender, RoutedEventArgs e)
        {
            var selected = ((JobDTO)JobsGrid.SelectedItem);
            if (selected == null)
                return;

            var job = new JobDTO
            {
                Id = manager.NewJobId,
                Labor = selected.Labor,
                Name = selected.Name,
                VariantName = selected.VariantName,
                ProcessNames = selected.ProcessNames.ToList(),
                Skill = selected.Skill
            };

            Window win = new JobEditorWindow(job);

            win.ShowDialog();
            JobsGrid.ItemsSource = manager.Jobs.Values.ToList();
            JobsGrid.Items.Refresh();
        }

        private void EditJob(object sender, RoutedEventArgs e)
        {
            var selected = ((JobDTO)JobsGrid.SelectedItem);
            if (selected == null)
                return;

            Window win = new JobEditorWindow(selected);

            win.ShowDialog();
            JobsGrid.ItemsSource = manager.Jobs.Values.ToList();
            JobsGrid.Items.Refresh();
        }

        private void DeleteJob(object sender, RoutedEventArgs e)
        {
            var selected = ((JobDTO)JobsGrid.SelectedItem);
            if (selected == null)
                return;

            manager.Jobs.Remove(selected.Id);

            jobs = manager.Jobs.Values.ToList();
            JobsGrid.ItemsSource = jobs;
        }

        private void SaveJobs(object sender, RoutedEventArgs e)
        {
            manager.SaveJobs(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonJobs.json");
        }
    }
}
