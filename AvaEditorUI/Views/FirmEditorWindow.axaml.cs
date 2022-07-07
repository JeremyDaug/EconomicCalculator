using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects;

namespace AvaEditorUI.Views;

public partial class FirmEditorWindow : Window
{
    public FirmEditorWindow()
    {
        DataContext = new FirmEditorViewModel(this);
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public FirmEditorWindow(FirmModel original)
    {
        DataContext = new FirmEditorViewModel(original, this);
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