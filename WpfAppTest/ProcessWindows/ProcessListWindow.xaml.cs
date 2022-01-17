using EconomicCalculator;
using EconomicCalculator.DTOs.Processes;
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

namespace EditorInterface.ProcessWindows
{
    /// <summary>
    /// Interaction logic for ProcessListWindow.xaml
    /// </summary>
    public partial class ProcessListWindow : Window
    {
        private DTOManager manager;

        public ProcessListWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            ProcessGrid.ItemsSource = manager.Processes.Values;
        }

        private void NewProcess(object sender, RoutedEventArgs e)
        {
            var newProcess = new ProcessDTO();

            newProcess.Id = manager.NewProcessId;

            Window win = new ProcessWindow(newProcess);
            win.ShowDialog();

            ProcessGrid.ItemsSource = manager.Processes.Values;
            ProcessGrid.Items.Refresh();
        }

        private void EditProcess(object sender, RoutedEventArgs e)
        {
            var selected = (ProcessDTO)ProcessGrid.SelectedItem;

            if (selected == null)
                return;

            Window win = new ProcessWindow(selected);

            win.ShowDialog();

            ProcessGrid.ItemsSource = manager.Processes.Values;
            ProcessGrid.Items.Refresh();
        }

        private void CopyProcess(object sender, RoutedEventArgs e)
        {
            var selected = (ProcessDTO)ProcessGrid.SelectedItem;

            if (selected == null)
                return;

            var copy = new ProcessDTO(selected);
            copy.Id = manager.NewProcessId;

            ProcessWindow win = new ProcessWindow(copy);

            win.ShowDialog();

            ProcessGrid.ItemsSource = manager.Processes.Values;
            ProcessGrid.Items.Refresh();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            manager.SaveProcesses(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonProcesses.json");
        }

        private void LoadFromFile(object sender, RoutedEventArgs e)
        {
            // TODO, load button.
        }
    }
}
