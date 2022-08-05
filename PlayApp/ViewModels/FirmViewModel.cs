using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using AvaEditorUI.Models;
using Avalonia.Controls;
using EconomicSim.Enums;
using EconomicSim.Objects;
using EconomicSim.Objects.Firms;
using PlayApp.Helpers;
using PlayApp.Views;
using ReactiveUI;
using ProductTag = EconomicSim.Objects.Products.ProductTags.ProductTag;

namespace PlayApp.ViewModels;

public class FirmViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private FirmModel model;
    private Firm original;
    private Window _window;
    private string _name;
    private int _selectedTech;
    private int _selectedRegion;
    private string _headquarters;
    private Pair<string, decimal>? _selectedResource;
    private string _firmRank;
    private string _ownershipStructure;
    private string _profitStructure;
    private string _parent;
    private int _selectedChild;
    private int _selectedJob;
    private Pair<string, decimal>? _selectedProduct;
    private bool _debugMode;
    private string _pricingUnit;
    private bool _canChangePrice;
    private decimal _priceIncrement = 1;
    private decimal _resourceIncrement = 1;
    private decimal _wageIncrement;
    private bool _canViewBudget;

    public FirmViewModel()
    {
        DebugMode = true;
        CanChangePrice = true;
    }

    public FirmViewModel(Firm original, Window window)
    {
        model = new FirmModel(original);
        _window = window;
        this.original = original;
        // TODO include other playmode checks and check for player ownership.
        CanChangePrice = dc.DebugMode;
        CanViewBudget = dc.DebugMode;

        DebugMode = dc.DebugMode;

        Name = original.Name;
        FirmRank = EnumExtension.ToName(original.FirmRank);
        OwnershipStructure = EnumExtension.ToName(original.OwnershipStructure);
        ProfitStructure = EnumExtension.ToName(original.ProfitStructure);
        if (original.Parent != null)
            Parent = original.Parent.Name;

        Children = new ObservableCollection<string>();
        AddChildren(original);

        Headquarters = original.HeadQuarters.Name;

        IncreasePrice = ReactiveCommand.Create(_increasePrice);
        ReducePrice = ReactiveCommand.Create(_reducePrice);
        IncreaseResource = ReactiveCommand.Create(_increaseResource);
        ReduceResource = ReactiveCommand.Create(_reduceResource);
        ViewOperations = ReactiveCommand.Create(_viewOperations);

        PricingOptions = new ObservableCollection<string>();
        // select products being used as currencies
        foreach (var option in dc.Products.Values
                     .Where(x => x.ProductTags.Any(y => y.tag == ProductTag.Currency)))
        {
            PricingOptions.Add(option.GetName());
        }
        foreach (var option in dc.Products.Values
                     .Where(x => x.ProductTags.All(y => y.tag != ProductTag.Currency))
                     .Where(x => original.HeadQuarters.GetMarketPrice.ContainsKey(x)))
        {
            PricingOptions.Add(option.GetName());
        }
        // Select Unit at the start based on market/government's currency
        
        Products = new ObservableCollection<Pair<string, decimal>>();
        foreach (var product in original.Products)
            Products.Add(new Pair<string, decimal>(product.Key.GetName(), product.Value));
        
        Resources = new ObservableCollection<Pair<string, decimal>>();
        foreach (var resource in original.Resources)
            Resources.Add(new Pair<string, decimal>(resource.Key.GetName(), resource.Value));

        Jobs = new ObservableCollection<FirmJobModel>();
        foreach (var job in original.Jobs)
            Jobs.Add(new FirmJobModel
            {
                Job = job.Job.GetName(),
                Wage = job.Wage,
                WageType = EnumExtension.ToName(job.WageType)
            });

        WageOptions = new ObservableCollection<string>();
        foreach (var wage in Enum.GetValues(typeof(WageType)))
            WageOptions.Add(EnumExtension.ToName((WageType)wage));
        
        IncrementOptions = new ObservableCollection<decimal>();
        IncrementOptions.Add(0.001m);
        IncrementOptions.Add(0.01m);
        IncrementOptions.Add(0.1m);
        IncrementOptions.Add(1m);
        IncrementOptions.Add(10m);
        IncrementOptions.Add(100m);
        IncrementOptions.Add(1000m);
        
        PricingUnit = PricingOptions.First();
    }
    
    public ReactiveCommand<Unit, Unit> IncreasePrice { get; set; }
    public ReactiveCommand<Unit, Unit> ReducePrice { get; set; }
    public ReactiveCommand<Unit, Unit> IncreaseResource { get; set; }
    public ReactiveCommand<Unit, Unit> ReduceResource { get; set; }
    public ReactiveCommand<Unit, Unit> ViewOperations { get; set; }

    private void _viewOperations()
    {
        if (BudgetWindow != null)
            return;

        BudgetWindow = new FirmOperationsWindow(original);
        // connect events
        BudgetWindow.Show(_window);
        BudgetWindow.Closed += budgetClosed;
    }

    private void budgetClosed(object sender, EventArgs e)
    {
        BudgetWindow = null;
    }
    
    private void AddChildren(Firm original)
    {
        foreach (var child in original.Children)
        {
            Children.Add(child.Name);
            AddChildren(child);
        }
    }
    
    private void _increasePrice()
    {
        if (SelectedProduct == null)
            return;

        SelectedProduct.Secondary += PriceIncrement;
    }

    private void _reducePrice()
    {
        if (SelectedProduct == null)
            return;

        SelectedProduct.Secondary -= PriceIncrement;
    }

    private void _increaseResource()
    {
        if (SelectedResource == null)
            return;

        SelectedResource.Secondary += ResourceIncrement;
    }

    private void _reduceResource()
    {
        if (SelectedResource == null)
            return;

        SelectedResource.Secondary -= ResourceIncrement;
    }
    
    public Window? BudgetWindow { get; set; }
    
    public ObservableCollection<string> WageOptions { get; set; }
    public ObservableCollection<decimal> IncrementOptions { get; set; }

    public decimal WageIncrement
    {
        get => _wageIncrement;
        set => this.RaiseAndSetIfChanged(ref _wageIncrement, value);
    }

    public decimal PriceIncrement
    {
        get => _priceIncrement;
        set => this.RaiseAndSetIfChanged(ref _priceIncrement, value);
    }
    
    public decimal ResourceIncrement
    {
        get => _resourceIncrement;
        set => this.RaiseAndSetIfChanged(ref _resourceIncrement, value);
    }

    public bool CanChangePrice
    {
        get => _canChangePrice;
        set => this.RaiseAndSetIfChanged(ref _canChangePrice, value);
    }

    public bool CanViewBudget
    {
        get => _canViewBudget;
        set => this.RaiseAndSetIfChanged(ref _canViewBudget, value);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string FirmRank
    {
        get => _firmRank;
        set => this.RaiseAndSetIfChanged(ref _firmRank, value);
    }

    public string OwnershipStructure
    {
        get => _ownershipStructure;
        set => this.RaiseAndSetIfChanged(ref _ownershipStructure, value);
    }

    public string ProfitStructure
    {
        get => _profitStructure;
        set => this.RaiseAndSetIfChanged(ref _profitStructure, value);
    }

    public string Parent
    {
        get => _parent;
        set => this.RaiseAndSetIfChanged(ref _parent, value);
    }

    public ObservableCollection<string> Children { get; set; }

    public int SelectedChild
    {
        get => _selectedChild;
        set => this.RaiseAndSetIfChanged(ref _selectedChild, value);
    }

    public ObservableCollection<FirmJobModel> Jobs { get; set; }

    public int SelectedJob
    {
        get => _selectedJob;
        set => this.RaiseAndSetIfChanged(ref _selectedJob, value);
    }

    public string PricingUnit
    {
        get => _pricingUnit;
        set
        {
            this.RaiseAndSetIfChanged(ref _pricingUnit, value);
            updatePrices();
        }
    }

    private void updatePrices()
    {
        // get average market prices
        var MarketPrices = original.HeadQuarters.GetMarketPrice;
        var unitProduct = dc.Products[PricingUnit];
        if (!MarketPrices.ContainsKey(unitProduct))
            return; // if it doesn't have a price don't calculate.
        Products.Clear();
        foreach (var price in original.Products)
        {
            Products.Add(new Pair<string, decimal>(price.Key.GetName(),
                price.Value / MarketPrices[unitProduct]));
        }
    }

    public ObservableCollection<string> PricingOptions { get; set; }

    public ObservableCollection<Pair<string, decimal>> Products { get; set; }

    public Pair<string, decimal>? SelectedProduct
    {
        get => _selectedProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedProduct, value);
    }

    public ObservableCollection<Pair<string, decimal>> Resources { get; set; }

    public Pair<string, decimal>? SelectedResource
    {
        get => _selectedResource;
        set => this.RaiseAndSetIfChanged(ref _selectedResource, value);
    }

    public string Headquarters
    {
        get => _headquarters;
        set => this.RaiseAndSetIfChanged(ref _headquarters, value);
    }

    public ObservableCollection<string> OperatingRegions { get; set; }

    public int SelectedRegion
    {
        get => _selectedRegion;
        set => this.RaiseAndSetIfChanged(ref _selectedRegion, value);
    }

    public ObservableCollection<string> Techs { get; set; }

    public int SelectedTech
    {
        get => _selectedTech;
        set => this.RaiseAndSetIfChanged(ref _selectedTech, value);
    }

    public bool DebugMode
    {
        get => _debugMode;
        set => this.RaiseAndSetIfChanged(ref _debugMode, value);
    }
}