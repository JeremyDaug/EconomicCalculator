using AvaEditorUI.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

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

        public void GotoTechs(object? sender, RoutedEventArgs e)
        {
            var win = new TechListEditorWindow();
            win.Show();
            this.Close();
        }

        private void GotoProducts(object? sender, RoutedEventArgs e)
        {
            var win = new ProductListWindow();
            win.Show();
            this.Close();
        }

        private void GoToProcesses(object? sender, RoutedEventArgs e)
        {
            var win = new ProcessListWindow();
            win.Show();
            this.Close();
        }

        private void GoToJobs(object? sender, RoutedEventArgs e)
        {
            var win = new JobListWindow();
            win.Show();
            this.Close();
        }
        
        private void GoToSpecies(object? sender, RoutedEventArgs e)
        {
            var win = new SpeciesListWindow();
            win.Show();
            this.Close();
        }
    }
}