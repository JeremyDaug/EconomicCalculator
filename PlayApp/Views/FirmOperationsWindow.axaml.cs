using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects.Firms;
using PlayApp.ViewModels;

namespace PlayApp.Views;

public partial class FirmOperationsWindow : Window
{
    public FirmOperationsWindow()
    {
        InitializeComponent();

        DataContext = new FirmOperationsViewModel();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public FirmOperationsWindow(Firm parent)
    {
        InitializeComponent();

        DataContext = new FirmOperationsViewModel(parent, this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}