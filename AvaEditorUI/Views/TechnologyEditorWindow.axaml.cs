using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class TechnologyEditorWindow : Window
{
    public TechnologyEditorWindow()
    {
        DataContext = new TechnologyEditorViewModel(this);
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public TechnologyEditorWindow(TechnologyEditorModel model)
    {
        DataContext = new TechnologyEditorViewModel(model, this);
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