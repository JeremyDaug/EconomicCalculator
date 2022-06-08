using System.Collections.Generic;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EconomicSim.Objects.Products.ProductTags;

namespace AvaEditorUI.Views;

public partial class ProductTagWindow : Window
{
    public ProductTagViewModel vm;
    public ProductTagWindow()
    {
        DataContext = vm = new ProductTagViewModel(this);
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public ProductTagWindow(ProductTag original, Dictionary<string, object>? data)
    {
        DataContext = vm = new ProductTagViewModel(original, data, this);
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