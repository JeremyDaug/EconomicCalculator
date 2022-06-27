using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects.Processes;

namespace AvaEditorUI.Views;

public partial class ProcessWantEditorWindow : Window
{
    public ProcessWantEditorViewModel vm;
    
    public ProcessWantEditorWindow()
    {
        InitializeComponent();

        DataContext = vm =  new ProcessWantEditorViewModel(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public ProcessWantEditorWindow(ProcessPartTag part)
    {
        InitializeComponent();

        DataContext = vm =  new ProcessWantEditorViewModel(part, this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public ProcessWantEditorWindow(ProcessWantModel old)
    {
        InitializeComponent();

        DataContext = vm =  new ProcessWantEditorViewModel(old, this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}