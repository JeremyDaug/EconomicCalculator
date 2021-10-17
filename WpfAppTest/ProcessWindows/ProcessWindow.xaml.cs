using EconomicCalculator;
using EconomicCalculator.Storage.Processes;
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
    /// Interaction logic for ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        private Manager manager;
        private Process process;

        public ProcessWindow(Process process)
        {
            InitializeComponent();

            manager = Manager.Instance;

            this.process = process;

            ProcessId.Text = process.Id.ToString();
            ProcessName.Text = process.Name;
            VariantName.Text = process.VariantName;
            MinimumTime.Text = process.MinimumTime.ToString();
            ProcessSkill.Text = process.SkillName;
            MinimumLevel.Text = process.SkillMinimum.ToString();
            Maximum.Text = process.SkillMaximum.ToString();

            InputProductGrid.ItemsSource = process.InputProducts;
            InputWantGrid.ItemsSource = process.InputWants;
            CapitalProductGrid.ItemsSource = process.CapitalProducts;
            CapitalWantGrid.ItemsSource = process.CapitalWants;
            OutputProductGrid.ItemsSource = process.OutputProducts;
            OutputWantGrid.ItemsSource = process.OutputWants;
        }

        private void ProcessSkill_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #region Inputs

        private void NewInputProduct(object sender, RoutedEventArgs e)
        {
            NewProduct(ProcessSection.Input);
        }

        private void DeleteInputProduct(object sender, RoutedEventArgs e)
        {

        }

        private void NewInputWant(object sender, RoutedEventArgs args)
        {

        }

        private void DeleteInputWant(object sender, RoutedEventArgs args)
        {

        }

        #endregion Inputs
        #region Capital

        private void NewCapitalProduct(object sender, RoutedEventArgs e)
        {
            NewProduct(ProcessSection.Capital);
        }

        private void DeleteCapitalProduct(object sender, RoutedEventArgs e)
        {

        }

        private void NewCapitalWant(object sender, RoutedEventArgs args)
        {

        }

        private void DeleteCapitalWant(object sender, RoutedEventArgs args)
        {

        }

        #endregion Capital
        #region Output

        private void NewOutputProduct(object sender, RoutedEventArgs e)
        {
            NewProduct(ProcessSection.Output);
        }

        private void DeleteOutputProduct(object sender, RoutedEventArgs e)
        {

        }

        private void NewOutputWant(object sender, RoutedEventArgs args)
        {

        }

        private void DeleteOutputWant(object sender, RoutedEventArgs args)
        {

        }

        #endregion Output

        private void NewProduct(ProcessSection sec)
        {
            var prod = new ProcessProduct();

            var win = new ProcessProductWindow(prod, sec);

            win.ShowDialog();

            if (!win.ViewModel.ValidCommited)
            { // if commit never hit, don't add it.
                return;
            }

            switch (sec)
            {
                case ProcessSection.Input:
                    process.InputProducts.Add(win.ViewModel.ProcessProduct);
                    break;
                case ProcessSection.Capital:
                    process.CapitalProducts.Add(win.ViewModel.ProcessProduct);
                    break;
                case ProcessSection.Output:
                    process.OutputProducts.Add(win.ViewModel.ProcessProduct);
                    break;
            }

            RefreshGrids();
        }

        private void EditProduct(ProcessProduct prod, ProcessSection sec)
        {
            var win = new ProcessProductWindow(prod, sec);

            win.ShowDialog();

            if (!win.ViewModel.ValidCommited)
            {
                return;
            }
            switch (sec)
            {
                case ProcessSection.Input:
                    process.InputProducts.Remove(prod);
                    process.InputProducts.Add(win.ViewModel.ProcessProduct);
                    break;
                case ProcessSection.Capital:
                    process.CapitalProducts.Remove(prod);
                    process.CapitalProducts.Add(win.ViewModel.ProcessProduct);
                    break;
                case ProcessSection.Output:
                    process.OutputProducts.Remove(prod);
                    process.OutputProducts.Add(win.ViewModel.ProcessProduct);
                    break;
            }

            prod = win.ViewModel.ProcessProduct;

            RefreshGrids();
        }

        private void RefreshGrids()
        {
            InputProductGrid.ItemsSource = process.InputProducts;
            CapitalProductGrid.ItemsSource = process.CapitalProducts;
            OutputProductGrid.ItemsSource = process.OutputProducts;
            InputWantGrid.ItemsSource = process.InputWants;
            OutputWantGrid.ItemsSource = process.OutputWants;
            CapitalWantGrid.ItemsSource = process.CapitalWants;

            InputProductGrid.Items.Refresh();
            CapitalProductGrid.Items.Refresh();
            OutputProductGrid.Items.Refresh();
            InputWantGrid.Items.Refresh();
            OutputWantGrid.Items.Refresh();
            CapitalWantGrid.Items.Refresh();
        }

        private void InputProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProduct)InputProductGrid.SelectedItem;

            EditProduct(selection, ProcessSection.Input);
        }

        private void InputWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void CapitalProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProduct)CapitalProductGrid.SelectedItem;

            EditProduct(selection, ProcessSection.Capital);
        }

        private void CapitalWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void OutputProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProduct)OutputProductGrid.SelectedItem;

            EditProduct(selection, ProcessSection.Output);
        }

        private void OutputWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
