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
    /// Interaction logic for JobsListWindow.xaml
    /// </summary>
    public partial class JobsListWindow : Window
    {
        public IList<IJob> jobs;

        private Manager manager;

        public JobsListWindow()
        {
            InitializeComponent();

            manager = Manager.Instance;

            jobs = manager.Jobs.Values.ToList();

            JobsGrid.ItemsSource = jobs;
        }

        private void JobsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
