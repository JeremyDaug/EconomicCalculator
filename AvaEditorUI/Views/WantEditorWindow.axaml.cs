using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects.Wants;

namespace AvaEditorUI.Views;

public partial class WantEditorWindow : Window
{
    public WantEditorViewModel vm;
    
    public WantEditorWindow()
    {
        vm = new WantEditorViewModel(new Want());
        vm.Parent = this;
        DataContext = vm;
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public WantEditorWindow(IWant selection)
    {
        vm = new WantEditorViewModel(selection);
        vm.Parent = this;
        DataContext = vm;
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