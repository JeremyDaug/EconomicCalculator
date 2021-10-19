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
            DeleteProduct(ProcessSection.Input);
        }

        private void NewInputWant(object sender, RoutedEventArgs args)
        {
            NewWant(ProcessSection.Input);
        }

        private void DeleteInputWant(object sender, RoutedEventArgs args)
        {
            DeleteWant(ProcessSection.Input);
        }

        #endregion Inputs
        #region Capital

        private void NewCapitalProduct(object sender, RoutedEventArgs e)
        {
            NewProduct(ProcessSection.Capital);
        }

        private void DeleteCapitalProduct(object sender, RoutedEventArgs e)
        {
            DeleteProduct(ProcessSection.Capital);
        }

        private void NewCapitalWant(object sender, RoutedEventArgs args)
        {
            NewWant(ProcessSection.Capital);
        }

        private void DeleteCapitalWant(object sender, RoutedEventArgs args)
        {
            DeleteWant(ProcessSection.Capital);
        }

        #endregion Capital
        #region Output

        private void NewOutputProduct(object sender, RoutedEventArgs e)
        {
            NewProduct(ProcessSection.Output);
        }

        private void DeleteOutputProduct(object sender, RoutedEventArgs e)
        {
            DeleteProduct(ProcessSection.Output);
        }

        private void NewOutputWant(object sender, RoutedEventArgs args)
        {
            NewWant(ProcessSection.Output);
        }

        private void DeleteOutputWant(object sender, RoutedEventArgs args)
        {
            DeleteWant(ProcessSection.Output);
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

            RefreshGrids();
        }

        private void DeleteProduct(ProcessSection sec)
        {
            ProcessProduct selected = null;

            switch (sec)
            {
                case ProcessSection.Input:
                    selected = (ProcessProduct)InputProductGrid.SelectedItem;
                    break;
                case ProcessSection.Capital:
                    selected = (ProcessProduct)CapitalProductGrid.SelectedItem;
                    break;
                case ProcessSection.Output:
                    selected = (ProcessProduct)OutputProductGrid.SelectedItem;
                    break;
            }

            if (selected == null)
                return;

            switch (sec)
            {
                case ProcessSection.Input:
                    process.InputProducts.Remove(selected);
                    break;
                case ProcessSection.Capital:
                    process.CapitalProducts.Remove(selected);
                    break;
                case ProcessSection.Output:
                    process.OutputProducts.Remove(selected);
                    break;
            }

            RefreshGrids();
        }

        private void NewWant(ProcessSection sec)
        {
            var want = new ProcessWant();

            var win = new ProcessWantWindow(want, sec);

            win.ShowDialog();

            if (!win.ViewModel.ValidCommited)
            {// if comit was never hit, return.
                return;
            }

            switch (sec)
            {
                case ProcessSection.Input:
                    process.InputWants.Add(win.ViewModel.ProcessWant);
                    break;
                case ProcessSection.Capital:
                    process.InputWants.Add(win.ViewModel.ProcessWant);
                    break;
                case ProcessSection.Output:
                    process.InputWants.Add(win.ViewModel.ProcessWant);
                    break;
            }

            RefreshGrids();
        }

        private void EditWant(ProcessWant want, ProcessSection sec)
        {
            var win = new ProcessWantWindow(want, sec);

            win.ShowDialog();

            if (!win.ViewModel.ValidCommited)
            {
                return;
            }

            switch (sec)
            {
                case ProcessSection.Input:
                    process.InputWants.Remove(want);
                    process.InputWants.Add(win.ViewModel.ProcessWant);
                    break;
                case ProcessSection.Capital:
                    process.CapitalWants.Remove(want);
                    process.CapitalWants.Add(win.ViewModel.ProcessWant);
                    break;
                case ProcessSection.Output:
                    process.OutputWants.Remove(want);
                    process.OutputWants.Add(win.ViewModel.ProcessWant);
                    break;
            }

            RefreshGrids();
        }

        private void DeleteWant(ProcessSection sec)
        {
            ProcessWant selected = null;

            switch (sec)
            {
                case ProcessSection.Input:
                    selected = (ProcessWant)InputWantGrid.SelectedItem;
                    break;
                case ProcessSection.Capital:
                    selected = (ProcessWant)CapitalWantGrid.SelectedItem;
                    break;
                case ProcessSection.Output:
                    selected = (ProcessWant)OutputWantGrid.SelectedItem;
                    break;
            }

            if (selected == null)
                return;

            switch (sec)
            {
                case ProcessSection.Input:
                    process.InputWants.Remove(selected);
                    break;
                case ProcessSection.Capital:
                    process.CapitalWants.Remove(selected);
                    break;
                case ProcessSection.Output:
                    process.OutputWants.Remove(selected);
                    break;
            }

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
            var selection = (ProcessWant)InputWantGrid.SelectedItem;

            EditWant(selection, ProcessSection.Input);
        }

        private void CapitalProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProduct)CapitalProductGrid.SelectedItem;

            EditProduct(selection, ProcessSection.Capital);
        }

        private void CapitalWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessWant)CapitalWantGrid.SelectedItem;

            EditWant(selection, ProcessSection.Capital);
        }

        private void OutputProductGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessProduct)OutputProductGrid.SelectedItem;

            EditProduct(selection, ProcessSection.Output);
        }

        private void OutputWantGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selection = (ProcessWant)OutputWantGrid.SelectedItem;

            EditWant(selection, ProcessSection.Output);
        }
    }
}
