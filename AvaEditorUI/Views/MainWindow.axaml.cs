using AvaEditorUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EconomicSim.Objects;

namespace AvaEditorUI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainWindowViewModel();
            DataContext = vm;
        }

        private void GotoWants(object? sender, RoutedEventArgs e)
        {
            var win = new WantListWindow();
            win.Show();
            this.Close();
        }

        private void GotoSkills(object? sender, RoutedEventArgs e)
        {
            var win = new SkillListsWindow();
            win.Show();
            this.Close();
        }
    }
}