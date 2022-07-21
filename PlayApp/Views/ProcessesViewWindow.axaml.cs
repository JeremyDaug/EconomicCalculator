using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects.Processes;
using PlayApp.ViewModels;

namespace PlayApp.Views;

public partial class ProcessesViewWindow : Window
{
    public ProcessesViewWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public ProcessesViewWindow(IProcess process)
    {
        InitializeComponent();

        Name = process.GetName();
        DataContext = new ProcessViewModel(process, this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}