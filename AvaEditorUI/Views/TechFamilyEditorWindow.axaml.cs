using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class TechFamilyEditorWindow : Window
{
    public TechFamilyEditorWindow()
    {
        InitializeComponent();

        DataContext = new TechFamilyEditorViewModel(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public TechFamilyEditorWindow(TechFamilyEditorModel selection)
    {
        InitializeComponent();

        DataContext = new TechFamilyEditorViewModel(selection, this);
#if DEBUG
        this.AttachDevTools();
#endif
    } 

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}