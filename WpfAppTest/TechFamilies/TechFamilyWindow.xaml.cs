using EconomicSim;
using EconomicSim.DTOs.Technology;
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

namespace EditorInterface.TechFamilies
{
    /// <summary>
    /// Interaction logic for TechFamilyWindow.xaml
    /// </summary>
    public partial class TechFamilyWindow : Window
    {
        private TechFamilyDTO techFam;
        private DTOManager manager;
        private TechFamilyViewModel viewModel;

        public TechFamilyWindow(TechFamilyDTO techFam)
        {
            this.techFam = techFam;

            manager = DTOManager.Instance;

            InitializeComponent();

            viewModel = new TechFamilyViewModel(techFam);
            DataContext = viewModel;
        }

        private void CloseBind_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void AddTech(object sender, MouseButtonEventArgs e)
        {
            viewModel.AddTech();
        }

        private void RemoveTech(object sender, MouseButtonEventArgs e)
        {
            viewModel.RemoveTech();
        }

        private void AddRelation(object sender, MouseButtonEventArgs e)
        {
            viewModel.ShiftRelationToFamily();
        }

        private void RemoveRelation(object sender, MouseButtonEventArgs e)
        {
            viewModel.ShiftRelationFromFamily();
        }
    }
}
