using EconomicCalculator;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Processes;
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
        private Manager manager;
        private Process process;

        private List<IProcessProduct> inputProducts;
        private List<IProcessWant> inputWants;
        private List<IProcessProduct> capitalProducts;
        private List<IProcessWant> capitalWants;
        private List<IProcessProduct> outputProducts;
        private List<IProcessWant> outputWants;

        private ProcessViewModel viewModel;

        public ProcessWindow()
        {
            InitializeComponent();

            manager = Manager.Instance;

            process = new Process();
            process.Id = manager.NewProcessId;
            ImageSelected.Text = manager.DefaultIcon;
            ImageView.Source = new BitmapImage(new Uri(process.Icon));

            viewModel = new ProcessViewModel(process);
            DataContext = viewModel;
        }

        public ProcessWindow(Process process)
        {
            InitializeComponent();

            manager = Manager.Instance;

            this.process = process;

            if (File.Exists(process.Icon))
                ImageView.Source = new BitmapImage(new Uri(process.Icon));

            viewModel = new ProcessViewModel(process);
            DataContext = viewModel;
        }
        
        #region EditDoubleClicks

        private void InputProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProduct)InputProductGrid.SelectedItem;

            viewModel.EditProduct(selection, ProcessSection.Input);
        }

        private void InputWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessWant)InputWantGrid.SelectedItem;

            viewModel.EditWant(selection, ProcessSection.Input);
        }

        private void CapitalProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProduct)CapitalProductGrid.SelectedItem;

            viewModel.EditProduct(selection, ProcessSection.Capital);
        }

        private void CapitalWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessWant)CapitalWantGrid.SelectedItem;

            viewModel.EditWant(selection, ProcessSection.Capital);
        }

        private void OutputProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProduct)OutputProductGrid.SelectedItem;

            viewModel.EditProduct(selection, ProcessSection.Output);
        }

        private void OutputWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessWant)OutputWantGrid.SelectedItem;

            viewModel.EditWant(selection, ProcessSection.Output);
        }

        #endregion EditDoubleClicks
    }
}
