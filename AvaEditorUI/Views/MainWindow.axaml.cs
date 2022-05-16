using System.Collections.Generic;
using System.ComponentModel;
using AvaEditorUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DynamicData.Binding;
using EconomicSim.Objects;

namespace AvaEditorUI.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;
        private IDataContext dc = DataContextFactory.GetDataContext;
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainWindowViewModel();
            DataContext = vm;
        }

        private void GotoWants(object? sender, RoutedEventArgs e)
        {
            var win = new WantListWindow();
            win.Show();
            this.Close();
        }
    }
}