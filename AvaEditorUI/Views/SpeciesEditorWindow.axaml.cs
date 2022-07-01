using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class SpeciesEditorWindow : Window
{
    public SpeciesEditorWindow()
    {
        InitializeComponent();

        //DataContext = new SpeciesEditorViewModel(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public SpeciesEditorWindow(SpeciesModel original)
    {
        InitializeComponent();

        //DataContext = new SpeciesEditorViewModel(original, this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}