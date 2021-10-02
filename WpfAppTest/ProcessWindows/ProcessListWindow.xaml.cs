using EconomicCalculator;
using EconomicCalculator.Storage.Processes;
using EconomicCalculator.Storage.Processes.ProcessTags;
using EconomicCalculator.Storage.Processes.ProductionTags;
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
        private Manager manager;

        public ProcessListWindow()
        {
            InitializeComponent();

            manager = Manager.Instance;

            // test space for new processes and save/loading

            /*
            var proc = new Process
            {
                Id = 0,
                Name = "Wheat Milling",
                VariantName = "",
                MinimumTime = 0.05m,
                SkillId = 1,
                SkillName = "Lugging",
                SkillMinimum = 1,
                SkillMaximum = 3,
                Tags = new List<IAttachedProcessTag>
                {
                    new AttachedProcessTag
                    {
                        Tag = ProcessTag.Consumption
                    }
                },
                TagStrings = new List<string>
                {
                    "Consumption"
                },
                InputProducts = new List<IProcessProduct>
                {
                    new ProcessProduct
                    {
                        ProductId = 0,
                        Amount = 1
                    }
                },
                Outputs = new List<IProcessProduct>
                {
                    new ProcessProduct
                    { // Wheat Flour
                        ProductId = 1,
                        Amount = 0.7m
                    },
                    new ProcessProduct
                    { // Bio waste
                        ProductId = 2,
                        Amount = 0.3m
                    }
                },
                Icon = "ProductImages\\flour.png",
                Description = "Grind Wheat Grain into Flour"
            };

            manager.Processes[proc.Id] = proc;
            */
            manager.LoadProcesses(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonProcesses.json");

            ProcessGrid.ItemsSource = manager.Processes.Values;
        }

        private void NewProcess(object sender, RoutedEventArgs e)
        {

        }

        private void EditProcess(object sender, RoutedEventArgs e)
        {

        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {

        }

        private void LoadFromFile(object sender, RoutedEventArgs e)
        {

        }
    }
}
