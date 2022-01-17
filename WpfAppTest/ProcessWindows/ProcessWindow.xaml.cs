using EconomicCalculator;
using EconomicCalculator.DTOs.Processes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EditorInterface.ProcessWindows
{
    /// <summary>
    /// Interaction logic for ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        private DTOManager manager;
        private ProcessDTO process;

        private ProcessViewModel viewModel;

        public ProcessWindow()
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            process = new ProcessDTO();
            process.Id = manager.NewProcessId;
            ImageSelected.Text = manager.DefaultIcon;
            ImageView.Source = new BitmapImage(new Uri(process.Icon));

            viewModel = new ProcessViewModel(process);
            DataContext = viewModel;
        }

        public ProcessWindow(ProcessDTO process)
        {
            InitializeComponent();

            manager = DTOManager.Instance;

            this.process = process;

            if (File.Exists(process.Icon))
                ImageView.Source = new BitmapImage(new Uri(process.Icon));

            viewModel = new ProcessViewModel(process);
            DataContext = viewModel;
        }
        
        #region EditDoubleClicks

        private void InputProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProductDTO)InputProductGrid.SelectedItem;

            viewModel.EditProduct(selection, ProcessSection.Input);
        }

        private void InputWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessWantDTO)InputWantGrid.SelectedItem;

            viewModel.EditWant(selection, ProcessSection.Input);
        }

        private void CapitalProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProductDTO)CapitalProductGrid.SelectedItem;

            viewModel.EditProduct(selection, ProcessSection.Capital);
        }

        private void CapitalWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessWantDTO)CapitalWantGrid.SelectedItem;

            viewModel.EditWant(selection, ProcessSection.Capital);
        }

        private void OutputProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProductDTO)OutputProductGrid.SelectedItem;

            viewModel.EditProduct(selection, ProcessSection.Output);
        }

        private void OutputWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessWantDTO)OutputWantGrid.SelectedItem;

            viewModel.EditWant(selection, ProcessSection.Output);
        }

        #endregion EditDoubleClicks
    }
}
