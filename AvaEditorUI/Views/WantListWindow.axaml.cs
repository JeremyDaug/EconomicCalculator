using System.ComponentModel;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects;

namespace AvaEditorUI.Views;

public partial class WantListWindow : Window
{
    private IDataContext dataContext = DataContextFactory.GetDataContext;
    private WantListViewModel vm;
    
    public WantListWindow()
    {
        InitializeComponent();

        vm = new WantListViewModel();
        DataContext = vm;
        
#if DEBUG
        this.AttachDevTools();
#endif
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

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}