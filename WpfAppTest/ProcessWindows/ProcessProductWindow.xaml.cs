using EconomicCalculator;
using EconomicCalculator.DTOs.Processes;
using EconomicCalculator.DTOs.Processes.ProductionTags;
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
    /// Interaction logic for ProcessProductWindow.xaml
    /// </summary>
    public partial class ProcessProductWindow : Window
    {
        public ProcessProduct product;
        public ProcessProductViewModel ViewModel;

        public ProcessProductWindow(ProcessProduct procProd, ProcessSection section)
        {
            ViewModel = new ProcessProductViewModel(procProd, section);

            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
