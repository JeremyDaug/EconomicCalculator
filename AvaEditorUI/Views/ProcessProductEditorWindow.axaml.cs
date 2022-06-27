using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects.Processes;

namespace AvaEditorUI.Views;

public partial class ProcessProductEditorWindow : Window
{
    public ProcessProductEditorViewModel vm;
    public ProcessProductEditorWindow()
    {
        InitializeComponent();

        DataContext = vm = new ProcessProductEditorViewModel(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public ProcessProductEditorWindow(ProcessPartTag part)
    {
        InitializeComponent();
        
        DataContext = vm = new ProcessProductEditorViewModel(part, this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public ProcessProductEditorWindow(ProcessProductModel old)
    {
        InitializeComponent();
        
        DataContext = vm = new ProcessProductEditorViewModel(old, this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}