using System.ComponentModel;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class JobListWindow : Window
{
    public JobListWindow()
    {
        DataContext = new JobListViewModel(this);
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    protected override void OnClosing(CancelEventArgs e)
    {
        var win = new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };
        win.Show();
        base.OnClosing(e);
    }
}