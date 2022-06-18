using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Processes.ProcessTags;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class ProcessEditorViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window _window;
    private ProcessModel _original;
    private string _name = "";
    private string _variantName = "";
    private string _description = "";
    private decimal _minimumTime;
    private string _skill = "";
    private decimal _skillMin;
    private decimal _skillMax;
    private string _techRequirements = "";
    private ProcessProductModel? _selectedInputProduct;
    private ProcessProductModel? _selectedCapitalProduct;
    private ProcessProductModel? _selectedOutputProduct;
    private ProcessWantModel? _selectedInputWant;
    private ProcessWantModel? _selectedCapitalWant;
    private ProcessWantModel? _selectedOutputWant;
    private string _processTagToRemove = "";
    private string _processTagToAdd = "";
    private bool _failureEnabled;
    private bool _consumptionEnabled;
    private bool _maintenanceEnabled;
    private bool _useEnabled;
    private bool _chanceEnabled;
    private bool _cropEnabled;
    private bool _mineEnabled;
    private bool _extractorEnabled;
    private bool _tapEnabled;
    private bool _refinerEnabled;
    private bool _sorterEnabled;
    private bool _scrubberEnabled;
    private bool _scrappingEnabled;
    private bool _scrubber;
    private bool _scrapping;
    private bool _sorter;
    private bool _refiner;
    private bool _tap;
    private bool _extractor;
    private bool _mine;
    private bool _crop;
    private bool _chance;
    private bool _use;
    private bool _maintenance;
    private bool _consumption;
    private bool _failure;
    private bool _productSelectionEnabled;
    private string _selectedProduct = "";
    private bool _inputsEnabled;
    private bool _capitalsEnabled;

    public ProcessEditorViewModel()
    {
        _original = new ProcessModel();

        InputProducts = new ObservableCollection<ProcessProductModel>();
        CapitalProducts = new ObservableCollection<ProcessProductModel>();
        OutputProducts = new ObservableCollection<ProcessProductModel>();
        InputWants = new ObservableCollection<ProcessWantModel>();
        CapitalWants = new ObservableCollection<ProcessWantModel>();
        OutputWants = new ObservableCollection<ProcessWantModel>();
        
        ProcessTags = new ObservableCollection<string>();
        SkillOptions = new ObservableCollection<string>();
        TechOptions = new ObservableCollection<string>();
        ProcessTagOptions = new ObservableCollection<string>();
        ProductOptions = new ObservableCollection<string>(dc.Products.Keys);

        AddInputProduct = ReactiveCommand.Create(_addInputProduct);
        AddCapitalProduct = ReactiveCommand.Create(_addCapitalProduct);
        AddOutputProduct = ReactiveCommand.Create(_addOutputProduct);
        RemoveInputProduct = ReactiveCommand.Create(_removeInputProduct);
        RemoveCapitalProduct = ReactiveCommand.Create(_removeCapitalProduct);
        RemoveOutputProduct = ReactiveCommand.Create(_removeOutputProduct);
        
        AddInputWant = ReactiveCommand.Create(_addInputWant);
        AddCapitalWant = ReactiveCommand.Create(_addCapitalWant);
        AddOutputWant = ReactiveCommand.Create(_addOutputWant);
        RemoveInputWant = ReactiveCommand.Create(_removeInputWant);
        RemoveCapitalWant = ReactiveCommand.Create(_removeCapitalWant);
        RemoveOutputWant = ReactiveCommand.Create(_removeOutputWant);
    }

    public ProcessEditorViewModel(Window win) : this()
    {
        _window = win;
        
        // get all options for a new process
        foreach (var skill in dc.Skills.Keys)
            SkillOptions.Add(skill);
        foreach (var tech in dc.Technologies.Keys)
            TechOptions.Add(tech);
        foreach (var tag in Enum.GetNames(typeof(ProcessTag)))
            ProcessTagOptions.Add(tag);
        UpdateEnabledTags();
    }
    
    public ProcessEditorViewModel(ProcessModel model, Window win) : this()
    {
        _original = model;
        _window = win;

        Name = model.Name;
        VariantName = model.VariantName;
        Description = model.Description;
        MinimumTime = model.MinimumTime;
        Skill = model.Skill;
        SkillMin = model.SkillMin;
        SkillMax = model.SkillMax;
        TechRequirements = model.TechRequirement;
        
        // add products and wants
        InputProducts = new ObservableCollection<ProcessProductModel>(model.InputProducts);
        CapitalProducts = new ObservableCollection<ProcessProductModel>(model.CapitalProducts);
        OutputProducts = new ObservableCollection<ProcessProductModel>(model.OutputProducts);
        InputWants = new ObservableCollection<ProcessWantModel>(model.InputWants);
        CapitalWants = new ObservableCollection<ProcessWantModel>(model.CapitalWants);
        OutputWants = new ObservableCollection<ProcessWantModel>(model.OutputWants);

        ProcessTags = new ObservableCollection<string>(model.ProcessTags.Select(x => x.ToString()));
        
        // get all options for a new process
        foreach (var skill in dc.Skills.Keys
                     .Where(x => model.Skill != x))
            SkillOptions.Add(skill);
        foreach (var tech in dc.Technologies.Keys
                     .Where(x => x != model.TechRequirement))
            TechOptions.Add(tech);
        foreach (var tag in Enum.GetNames(typeof(ProcessTag))
                     .Where(x => !model.ProcessTags.Select(y => y.ToString()).Contains(x)))
            ProcessTagOptions.Add(tag);

        Failure = model.ProcessTags.Contains(ProcessTag.Failure);
        Consumption = model.ProcessTags.Contains(ProcessTag.Consumption);
        Maintenance = model.ProcessTags.Contains(ProcessTag.Maintenance);
        Use = model.ProcessTags.Contains(ProcessTag.Use);
        Chance = model.ProcessTags.Contains(ProcessTag.Chance);
        Crop = model.ProcessTags.Contains(ProcessTag.Crop);
        Mine = model.ProcessTags.Contains(ProcessTag.Mine);
        Extractor = model.ProcessTags.Contains(ProcessTag.Extractor);
        Tap = model.ProcessTags.Contains(ProcessTag.Tap);
        Refiner = model.ProcessTags.Contains(ProcessTag.Refiner);
        Sorter = model.ProcessTags.Contains(ProcessTag.Sorter);
        Scrubber = model.ProcessTags.Contains(ProcessTag.Scrubber);
        Scrapping = model.ProcessTags.Contains(ProcessTag.Scrapping);
        
        // if it has a selected product, select it appropriately.
        if (Failure || Maintenance || Consumption)
        {
            SelectedProduct = model.InputProducts.First().Product;
        }

        if (Maintenance)
            SelectedProduct = model.InputProducts.First().Product;
    }
    
    public ReactiveCommand<Unit, Task> AddInputProduct { get; set; }
    public ReactiveCommand<Unit, Task> AddCapitalProduct { get; set; }
    public ReactiveCommand<Unit, Task> AddOutputProduct { get; set; }
    public ReactiveCommand<Unit, Task> RemoveInputProduct { get; set; }
    public ReactiveCommand<Unit, Task> RemoveCapitalProduct { get; set; }
    public ReactiveCommand<Unit, Task> RemoveOutputProduct { get; set; }
    
    public ReactiveCommand<Unit, Task> AddInputWant { get; set; }
    public ReactiveCommand<Unit, Task> AddCapitalWant { get; set; }
    public ReactiveCommand<Unit, Task> AddOutputWant { get; set; }
    public ReactiveCommand<Unit, Task> RemoveInputWant { get; set; }
    public ReactiveCommand<Unit, Task> RemoveCapitalWant { get; set; }
    public ReactiveCommand<Unit, Task> RemoveOutputWant { get; set; }

    private async Task _addInputProduct(){}
    private async Task _addCapitalProduct(){}
    private async Task _addOutputProduct(){}
    private async Task _removeInputProduct(){}
    private async Task _removeCapitalProduct(){}
    private async Task _removeOutputProduct(){}
    
    private async Task _addInputWant(){}
    private async Task _addCapitalWant(){}
    private async Task _addOutputWant(){}
    private async Task _removeInputWant(){}
    private async Task _removeCapitalWant(){}
    private async Task _removeOutputWant(){}

    private void UpdateEnabledTags()
    {
        if (!UpdatingEnabledTags)
        {
            UpdatingEnabledTags = true;
            InputsEnabled = Failure || Consumption || Maintenance || Use;
            CapitalsEnabled = Failure || Consumption || Maintenance || Use;

            FailureEnabled = !(Maintenance || Use || Consumption);
            ConsumptionEnabled = !(Failure || Use || Maintenance);
            MaintenanceEnabled = !(Failure || Consumption || Use);
            UseEnabled = !(Failure || Consumption || Maintenance);
            //Chance
            CropEnabled = !(Mine || Extractor || Tap || Refiner || Sorter || Scrapping || Scrubber);
            MineEnabled = !(Crop || Extractor || Tap || Refiner || Sorter || Scrapping || Scrubber);
            ExtractorEnabled = !(Mine || Crop || Tap || Refiner || Sorter || Scrapping || Scrubber);
            TapEnabled = !(Mine || Extractor || Crop || Refiner || Sorter || Scrapping || Scrubber);
            RefinerEnabled = !(Mine || Extractor || Tap || Crop || Sorter || Scrapping || Scrubber);
            SorterEnabled = !(Mine || Extractor || Tap || Refiner || Crop || Scrapping || Scrubber);
            ScrappingEnabled = !(Mine || Extractor || Tap || Refiner || Sorter || Crop || Scrubber);
            ScrubberEnabled = !(Mine || Extractor || Tap || Refiner || Sorter || Scrapping || Crop);
            UpdatingEnabledTags = false;
        }
    }

    private bool UpdatingEnabledTags { get; set; }

    private void UpdateSelectedProduct()
    {
        if (Failure || Consumption || Maintenance || Use)
        { // if any of these are active
            // clear the parts
            InputProducts.Clear();
            InputWants.Clear();
            CapitalProducts.Clear();
            CapitalWants.Clear();

            if (string.IsNullOrEmpty(SelectedProduct))
            { // if a product is already selected, update it.
                UpdateSelectedProduct();
            }
        }
    }
    
    public bool Failure
    {
        get => _failure;
        set
        {
            this.RaiseAndSetIfChanged(ref _failure, value);
            UpdateEnabledTags();
            UpdateSelectedProduct();
        }
    }

    public bool Consumption
    {
        get => _consumption;
        set
        {
            this.RaiseAndSetIfChanged(ref _consumption, value);
            UpdateEnabledTags();
            UpdateSelectedProduct();
        }
    }

    public bool Maintenance
    {
        get => _maintenance;
        set
        {
            this.RaiseAndSetIfChanged(ref _maintenance, value);
            UpdateEnabledTags();
            UpdateSelectedProduct();
        }
    }

    public bool Use
    {
        get => _use;
        set
        {
            this.RaiseAndSetIfChanged(ref _use, value);
            UpdateEnabledTags();
            UpdateSelectedProduct();
        }
    }

    public bool Chance
    {
        get => _chance;
        set
        {
            this.RaiseAndSetIfChanged(ref _chance, value);
            UpdateEnabledTags();
        }
    }

    public bool Crop
    {
        get => _crop;
        set
        {
            this.RaiseAndSetIfChanged(ref _crop, value);
            UpdateEnabledTags();
        }
    }

    public bool Mine
    {
        get => _mine;
        set
        {
            this.RaiseAndSetIfChanged(ref _mine, value);
            UpdateEnabledTags();
        }
    }

    public bool Extractor
    {
        get => _extractor;
        set
        {
            this.RaiseAndSetIfChanged(ref _extractor, value);
            UpdateEnabledTags();
        }
    }

    public bool Tap
    {
        get => _tap;
        set
        {
            this.RaiseAndSetIfChanged(ref _tap, value);
            UpdateEnabledTags();
        }
    }

    public bool Refiner
    {
        get => _refiner;
        set
        {
            this.RaiseAndSetIfChanged(ref _refiner, value);
            UpdateEnabledTags();
        }
    }

    public bool Sorter
    {
        get => _sorter;
        set
        {
            this.RaiseAndSetIfChanged(ref _sorter, value);
            UpdateEnabledTags();
        }
    }

    public bool Scrapping
    {
        get => _scrapping;
        set
        {
            this.RaiseAndSetIfChanged(ref _scrapping, value);
            UpdateEnabledTags();
        }
    }

    public bool Scrubber
    {
        get => _scrubber;
        set
        {
            this.RaiseAndSetIfChanged(ref _scrubber, value);
            UpdateEnabledTags();
        }
    }

    #region Enableds
    
    public bool FailureEnabled
    {
        get => _failureEnabled;
        set => this.RaiseAndSetIfChanged(ref _failureEnabled, value);
    }

    public bool ConsumptionEnabled
    {
        get => _consumptionEnabled;
        set => this.RaiseAndSetIfChanged(ref _consumptionEnabled, value);
    }

    public bool MaintenanceEnabled
    {
        get => _maintenanceEnabled;
        set => this.RaiseAndSetIfChanged(ref _maintenanceEnabled, value);
    }

    public bool UseEnabled
    {
        get => _useEnabled;
        set => this.RaiseAndSetIfChanged(ref _useEnabled, value);
    }

    public bool ChanceEnabled
    {
        get => _chanceEnabled;
        set => this.RaiseAndSetIfChanged(ref _chanceEnabled, value);
    }

    public bool CropEnabled
    {
        get => _cropEnabled;
        set => this.RaiseAndSetIfChanged(ref _cropEnabled, value);
    }

    public bool MineEnabled
    {
        get => _mineEnabled;
        set => this.RaiseAndSetIfChanged(ref _mineEnabled, value);
    }

    public bool ExtractorEnabled
    {
        get => _extractorEnabled;
        set => this.RaiseAndSetIfChanged(ref _extractorEnabled, value);
    }

    public bool TapEnabled
    {
        get => _tapEnabled;
        set => this.RaiseAndSetIfChanged(ref _tapEnabled, value);
    }

    public bool RefinerEnabled
    {
        get => _refinerEnabled;
        set => this.RaiseAndSetIfChanged(ref _refinerEnabled, value);
    }

    public bool SorterEnabled
    {
        get => _sorterEnabled;
        set => this.RaiseAndSetIfChanged(ref _sorterEnabled, value);
    }

    public bool ScrubberEnabled
    {
        get => _scrubberEnabled;
        set => this.RaiseAndSetIfChanged(ref _scrubberEnabled, value);
    }
    
    public bool ScrappingEnabled
    {
        get => _scrappingEnabled;
        set => this.RaiseAndSetIfChanged(ref _scrappingEnabled, value);
    }
    
    public bool InputsEnabled
    {
        get => _inputsEnabled;
        set => this.RaiseAndSetIfChanged(ref _inputsEnabled, value);
    }

    public bool CapitalsEnabled
    {
        get => _capitalsEnabled;
        set => this.RaiseAndSetIfChanged(ref _capitalsEnabled, value);
    }
    
    #endregion

    #region  Properties
    
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

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public decimal MinimumTime
    {
        get => _minimumTime;
        set => this.RaiseAndSetIfChanged(ref _minimumTime, value);
    }

    public string Skill
    {
        get => _skill;
        set => this.RaiseAndSetIfChanged(ref _skill, value);
    }

    public decimal SkillMin
    {
        get => _skillMin;
        set => this.RaiseAndSetIfChanged(ref _skillMin, value);
    }

    public decimal SkillMax
    {
        get => _skillMax;
        set => this.RaiseAndSetIfChanged(ref _skillMax, value);
    }

    public string TechRequirements
    {
        get => _techRequirements;
        set => this.RaiseAndSetIfChanged(ref _techRequirements, value);
    }

    public ProcessProductModel SelectedInputProduct
    {
        get => _selectedInputProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedInputProduct, value);
    }
    
    public ProcessProductModel SelectedCapitalProduct
    {
        get => _selectedCapitalProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedCapitalProduct, value);
    }
    
    public ProcessProductModel SelectedOutputProduct
    {
        get => _selectedOutputProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedOutputProduct, value);
    }

    public ProcessWantModel SelectedInputWant
    {
        get => _selectedInputWant;
        set => this.RaiseAndSetIfChanged(ref _selectedInputWant, value);
    }

    public ProcessWantModel SelectedCapitalWant
    {
        get => _selectedCapitalWant;
        set => this.RaiseAndSetIfChanged(ref _selectedCapitalWant, value);
    }

    public ProcessWantModel SelectedOutputWant
    {
        get => _selectedOutputWant;
        set => this.RaiseAndSetIfChanged(ref _selectedOutputWant, value);
    }

    public string ProcessTagToRemove
    {
        get => _processTagToRemove;
        set => this.RaiseAndSetIfChanged(ref _processTagToRemove, value);
    }

    public string ProcessTagToAdd
    {
        get => _processTagToAdd;
        set => this.RaiseAndSetIfChanged(ref _processTagToAdd, value);
    }

    public bool ProductSelectionEnabled
    {
        get => _productSelectionEnabled;
        set => this.RaiseAndSetIfChanged(ref _productSelectionEnabled, value);
    }

    public string SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedProduct, value);
            UpdateSelectedProduct();
        }
    }
    
    #endregion

    public ObservableCollection<ProcessProductModel> InputProducts { get; set; }
    public ObservableCollection<ProcessProductModel> CapitalProducts { get; set; }
    public ObservableCollection<ProcessProductModel> OutputProducts { get; set; }
    
    public ObservableCollection<ProcessWantModel> InputWants { get; set; }
    public ObservableCollection<ProcessWantModel> CapitalWants { get; set; }
    public ObservableCollection<ProcessWantModel> OutputWants { get; set; }
    
    public ObservableCollection<string> ProcessTags { get; set; }

    public ObservableCollection<string> ProcessTagOptions { get; set; }
    public ObservableCollection<string> SkillOptions { get; set; }
    public ObservableCollection<string> TechOptions { get; set; }
    
    public ObservableCollection<string> ProductOptions { get; set; }
}