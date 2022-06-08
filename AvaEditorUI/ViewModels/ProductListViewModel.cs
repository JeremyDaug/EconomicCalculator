using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using MessageBox.Avalonia;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class ProductListViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    private ProductEditorModel? _selectedProduct;

    public ProductListViewModel()
    {
        Products = new List<ProductEditorModel>();
        foreach (var product in dc.Products.Values)
            Products.Add(new ProductEditorModel(product));
        NewProduct = ReactiveCommand.Create(CreateNewProduct);
        EditProduct = ReactiveCommand.Create(EditExistingProduct);
        Save = ReactiveCommand.Create(SaveProducts);
    }

    public ProductListViewModel(Window win) : this()
    {
        _window = win;
    }
    
    public ReactiveCommand<Unit, Task> NewProduct { get; set; }
    public ReactiveCommand<Unit, Task> EditProduct { get; set; }
    public ReactiveCommand<Unit, Task> Save { get; set; }

    private async Task CreateNewProduct()
    {
        var win = new ProductEditorWindow();
        await win.ShowDialog(_window);
        ReloadProducts();
    }

    private async Task EditExistingProduct()
    {
        if (SelectedProduct == null) return;
        var win = new ProductEditorWindow(SelectedProduct);
        await win.ShowDialog(_window);
        ReloadProducts();
    }

    private void ReloadProducts()
    {
        Products.Clear();
        foreach (var prod in dc.Products.Values)
            Products.Add(new ProductEditorModel(prod));
    }

    private async Task SaveProducts()
    {
        dc.SaveProducts();

        var box = MessageBoxManager.GetMessageBoxStandardWindow("Products Saved!",
            "Products have been successfully saved!");
        await box.ShowDialog(_window);
    }

    public ProductEditorModel? SelectedProduct
    {
        get => _selectedProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
    }

    public List<ProductEditorModel> Products { get; }
}