using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class ProductEditorWindow : Window
{
    public ProductEditorWindow()
    {
        InitializeComponent();

        DataContext = new ProductEditorViewModel(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public ProductEditorWindow(ProductEditorModel model)
    {
        InitializeComponent();

        DataContext = new ProductEditorViewModel(model, this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}