using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Helpers;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Products.ProductTags;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class ProductEditorViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    private ProductEditorModel _original;
    private string _name = "";
    private string _variantName = "";
    private string _unitName = "";
    private decimal _mass;
    private decimal _bulk;
    private bool _fractional;
    private string _icon = "";
    private string _technology = "";
    private string _failure = "";
    private string? _wantToAdd = "";
    private Pair<string, decimal>? _wantToRemove;
    private int? _selectedTag;

    public ProductEditorViewModel()
    {
        _original = new ProductEditorModel();
        Wants = new ObservableCollection<Pair<string, decimal>>();
        UseProcesses = new ObservableCollection<string>();
        ConsumptionProcesses = new ObservableCollection<string>();
        MaintenanceProcesses = new ObservableCollection<string>();
        AvailableTechnologies = new ObservableCollection<string>(dc.Technologies.Keys);
        AvailableWants = new ObservableCollection<string>(dc.Wants.Keys);
        ProductTags = new List<(string tag, Dictionary<string, object>? parameters)>();
        TagStrings = new ObservableCollection<string>();

        AddWant = ReactiveCommand.Create(AddSelectedWant);
        RemoveWant = ReactiveCommand.Create(RemoveSelectedWant);
        Commit = ReactiveCommand.Create(CommitProduct);

        AddTag = ReactiveCommand.Create(AddNewTag);
        EditTag = ReactiveCommand.Create(EditSelectedTag);
        RemoveTag = ReactiveCommand.Create(RemoveSelectedTag);
    }

    public ProductEditorViewModel(ProductEditorWindow win) : this()
    {
        _window = win;
    }

    public ProductEditorViewModel(ProductEditorModel model, ProductEditorWindow win) : this()
    {
        _window = win;
        _original = model;
        Name = model.Name;
        VariantName = model.VariantName;
        UnitName = model.UnitName;
        Mass = model.Mass;
        Bulk = model.Bulk;
        Fractional = model.Fractional;
        Icon = model.Icon;
        Technology = model.Technology;
        Failure = model.FailureProcess;
        foreach (var want in model.Wants)
        {
            Wants.Add(new Pair<string, decimal>(want.Primary, want.Secondary));
            AvailableWants.Remove(want.Primary);
        }
        foreach (var proc in model.MaintenanceProcesses)
            MaintenanceProcesses.Add(proc);
        foreach (var proc in model.ConsumptionProcesses)
            ConsumptionProcesses.Add(proc);
        foreach (var proc in model.UseProcesses)
            UseProcesses.Add(proc);
        foreach (var tag in model.ProductTags)
        {
            if (tag.parameters == null)
            {
                ProductTags.Add((tag.tag, null));
                TagStrings.Add(tag.tag + "()");
            }
            else
            {
                
                ProductTags.Add((tag.tag, new Dictionary<string, object>(tag.parameters)));
                var result = tag.tag + "(\n";
                foreach (var pair in tag.parameters)
                    result += $"    {pair.Key}:{pair.Value.ToString()},\n";
                result += ")";
                TagStrings.Add(result);
            }
        }
    }

    public ReactiveCommand<Unit, Unit> AddWant { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveWant { get; set; }
    public ReactiveCommand<Unit, Task> AddTag { get; set; }
    public ReactiveCommand<Unit, Task> EditTag { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveTag { get; set; }
    public ReactiveCommand<Unit, Task> Commit { get; set; }

    private void AddSelectedWant()
    {
        if (WantToAdd == null) return;
        Wants.Add(new Pair<string, decimal>(WantToAdd, 0));
        AvailableWants.Remove(WantToAdd);
    }

    private void RemoveSelectedWant()
    {
        if (_wantToRemove == null) return;
        AvailableWants.Add(_wantToRemove.Primary);
        Wants.Remove(_wantToRemove);
    }

    private async Task AddNewTag()
    {
        var tagWin = new ProductTagWindow();
        await tagWin.ShowDialog(_window);
        if (!tagWin.vm.IsSaved)
            return;
        ProductTags.Add((tagWin.vm.SelectedTag, tagWin.vm.ExportParameters));
        var newTag = TagToString(tagWin.vm.SelectedTag, tagWin.vm.ExportParameters);
        
        TagStrings.Add(newTag);
    }

    private async Task EditSelectedTag()
    {
        if (SelectedTag == null)
            return;
        var tagDataToChange = ProductTags[SelectedTag.Value];
        var tag = (ProductTag) Enum.Parse(typeof(ProductTag), tagDataToChange.tag);
        var tagWin = new ProductTagWindow(tag, tagDataToChange.parameters);
        await tagWin.ShowDialog(_window);
        if (!tagWin.vm.IsSaved) return;

        // remove old tag
        ProductTags.RemoveAt(SelectedTag.Value);
        TagStrings.RemoveAt(SelectedTag.Value);
        
        // add new tag back in.
        ProductTags.Add((tagWin.vm.SelectedTag, tagWin.vm.ExportParameters));
        var newTag = TagToString(ProductTags.Last().tag, ProductTags.Last().parameters);

        TagStrings.Add(newTag);
    }
    
    private void RemoveSelectedTag()
    {
        if (SelectedTag == null)
            return;
        TagStrings.RemoveAt(SelectedTag.Value);
        ProductTags.RemoveAt(SelectedTag.Value);
    }

    private string TagToString(string tag, Dictionary<string, object>? parameters)
    {
        var result = tag;
        if (parameters != null)
        {
            result += " (";
            foreach (var parameter in parameters)
            {
                result += $"\n    {parameter.Key}:{parameter.Value.ToString()},";
            }

            result = result.TrimEnd(',') + ")";
        }

        return result;
    }

    private async Task CommitProduct()
    {
        // check values
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Product must have a name.");
        if (string.IsNullOrWhiteSpace(UnitName))
            errors.Add("Product must have a unit name.");
        if (Wants.Any(x => x.Secondary == 0))
            errors.Add("Wants cannot be 0.");
        var nameCombo = string.IsNullOrWhiteSpace(VariantName) ? Name : $"{Name}({VariantName})";

        var oldCombo = string.IsNullOrWhiteSpace(_original.VariantName) ? _original.Name : $"{_original.Name}({_original.VariantName})";
        
        if (dc.Products.ContainsKey(nameCombo) && nameCombo != oldCombo)
            errors.Add("Product is a duplicate of an existing product.");

        // throw any errors
        if (errors.Any())
        {
            var failure = MessageBoxManager.GetMessageBoxStandardWindow("Errors Found.",
                "Errors Found:\n" + string.Join('\n', errors));
            await failure.ShowDialog(_window);
            return;
        }

        var oldProd = _original.Name.Any() ? dc.Products[oldCombo] : null;
        
        // Duplicate sanity check
        // TODO improve this to enforce actual duplicate rules, not just trust the user.
        var uniqueProducts = ProductTags.Select(x => x.tag).Distinct();
        if (uniqueProducts.Count() != ProductTags.Count())
        {
            var dupFound = MessageBoxManager.GetMessageBoxStandardWindow("Duplicate Tag Found!",
                "Duplicate Product Tag found, continue?", ButtonEnum.YesNo);
            if (await dupFound.ShowDialog(_window) == ButtonResult.No)
                return;
        }

        // add/replace/update processes.
        // since processes cannot be added, they either exist and need to 
        // be updated, or don't and we can skip.
        if (oldProd != null)
        {
            var oldName = oldProd.GetName();

            oldProd.Name = Name;
            oldProd.VariantName = VariantName;
            if (oldName != oldProd.GetName())
            {
                dc.Products.Remove(oldName);
                dc.Products[oldProd.GetName()] = oldProd;
            }
            oldProd.UnitName = UnitName;
            oldProd.Bulk = Bulk;
            oldProd.Mass = Mass;
            oldProd.Fractional = Fractional;
            if (Technology.Any())
                oldProd.TechRequirement = dc.Technologies[Technology];
            oldProd.Wants.Clear();
            foreach (var want in Wants)
                oldProd.Wants.Add((dc.Wants[want.Primary], want.Secondary));
            // Tags
            oldProd.ProductTags.Clear();
            foreach (var tag in ProductTags)
            {
                var tagEnum = (ProductTag) Enum.Parse(typeof(ProductTag), tag.tag);
                oldProd.ProductTags.Add((tagEnum, tag.parameters == null ? null : new Dictionary<string, object>(tag.parameters)));
            }
            _original = new ProductEditorModel(oldProd);
        }
        else
        {
            // everything else is done, let's GO!
            var newProd = new Product
            {
                Name = Name,
                VariantName = VariantName,
                UnitName = UnitName,
                Bulk = Bulk,
                Mass = Mass,
                Fractional = Fractional
            };
            // add tech if available
            if (Technology.Any())
                newProd.TechRequirement = dc.Technologies[Technology];
            // Add wants
            foreach (var want in Wants)
                newProd.Wants.Add((dc.Wants[want.Primary], want.Secondary));
            // Tags
            foreach (var tag in ProductTags)
            {
                var tagEnum = (ProductTag) Enum.Parse(typeof(ProductTag), tag.tag);
                newProd.ProductTags.Add((tagEnum, tag.parameters == null ? null : new Dictionary<string, object>(tag.parameters)));
            }
            dc.Products.Add(newProd.GetName(), newProd);
            _original = new ProductEditorModel(newProd);
        }
        
        var success = MessageBoxManager.GetMessageBoxStandardWindow("Product Committed!",
            "Product has been committed, be sure to save the data to file!");
        await success.ShowDialog(_window);
    }
    
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string VariantName
    {
        get => _variantName;
        set => this.RaiseAndSetIfChanged(ref _variantName, value);
    }

    public string UnitName
    {
        get => _unitName;
        set => this.RaiseAndSetIfChanged(ref _unitName, value);
    }

    public decimal Mass
    {
        get => _mass;
        set => this.RaiseAndSetIfChanged(ref _mass, value);
    }

    public decimal Bulk
    {
        get => _bulk;
        set => this.RaiseAndSetIfChanged(ref _bulk, value);
    }

    public bool Fractional
    {
        get => _fractional;
        set => this.RaiseAndSetIfChanged(ref _fractional, value);
    }

    public string Icon
    {
        get => _icon;
        set
        {
            this.RaiseAndSetIfChanged(ref _icon, value);
        }
    }

    public string Technology
    {
        get => _technology;
        set => this.RaiseAndSetIfChanged(ref _technology, value);
    }
    
    // Product Tags
    
    public ObservableCollection<Pair<string, decimal>> Wants { get; set; }

    public string? WantToAdd
    {
        get => _wantToAdd;
        set => this.RaiseAndSetIfChanged(ref _wantToAdd, value);
    }
    
    public Pair<string, decimal>? WantToRemove
    {
        get => _wantToRemove;
        set => this.RaiseAndSetIfChanged(ref _wantToRemove, value);
    }

    public string Failure
    {
        get => _failure;
        set => this.RaiseAndSetIfChanged(ref _failure, value);
    }

    public int? SelectedTag
    {
        get => _selectedTag;
        set => this.RaiseAndSetIfChanged(ref _selectedTag, value);
    }

    public ObservableCollection<string> UseProcesses { get; set; }
    
    public ObservableCollection<string> ConsumptionProcesses { get; set; }

    public ObservableCollection<string> MaintenanceProcesses { get; set; }

    public ObservableCollection<string> AvailableTechnologies { get; set; }
    
    public List<(string tag, Dictionary<string, object>? parameters)> ProductTags { get; set; }
    public ObservableCollection<string> TagStrings { get; set; }
    
    public ObservableCollection<string> AvailableWants { get; set; }
}