using EconDTOs.DTOs.Processes;
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
    /// Interaction logic for ProcessWantWindow.xaml
    /// </summary>
    public partial class ProcessWantWindow : Window
    {
        public ProcessWant product;
        public ProcessWantViewModel ViewModel;

        public ProcessWantWindow(ProcessWant procWant, ProcessSection section)
        {
            ViewModel = new ProcessWantViewModel(procWant, section);

            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}
