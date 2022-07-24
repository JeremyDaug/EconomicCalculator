using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects.Firms;
using PlayApp.ViewModels;

namespace PlayApp.Views;

public partial class FirmViewWindow : Window
{
    public FirmViewWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public FirmViewWindow(Firm firm)
    {
        DataContext = new FirmViewModel(firm, this);
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}