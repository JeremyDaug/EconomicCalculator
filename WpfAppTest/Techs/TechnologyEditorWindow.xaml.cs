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

namespace EditorInterface.Techs
{
    /// <summary>
    /// Interaction logic for TechnologyEditorWindow.xaml
    /// </summary>
    public partial class TechnologyEditorWindow : Window
    {
        TechnologyEditorViewModel viewModel;

        public TechnologyEditorWindow(ITechnologyDTO tech)
        {
            InitializeComponent();

            viewModel = new TechnologyEditorViewModel(tech);

            DataContext = viewModel;
        }

        private void RemoveFamily(object sender, MouseButtonEventArgs e)
        {
            viewModel.RemoveFamily();
        }

        private void RemoveParent(object sender, MouseButtonEventArgs e)
        {
            viewModel.RemoveParentFromTech();
        }

        private void RemoveChild(object sender, MouseButtonEventArgs e)
        {
            viewModel.RemoveChildFromTech();
        }
    }
}
