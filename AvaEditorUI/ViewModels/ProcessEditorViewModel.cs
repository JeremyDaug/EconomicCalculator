using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Processes.ProductionTags;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class ProcessEditorViewModel : ViewModelBase
{
    #region privateProps
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    private ProcessModel _original;
    private bool _updatingEnabledTags;
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
    #endregion

    public ProcessEditorViewModel()
    {
        _original = new ProcessModel();

        InputProducts = new ObservableCollection<ProcessProductModel>();
        CapitalProducts = new ObservableCollection<ProcessProductModel>();
        OutputProducts = new ObservableCollection<ProcessProductModel>();
        InputWants = new ObservableCollection<ProcessWantModel>();
        CapitalWants = new ObservableCollection<ProcessWantModel>();
        OutputWants = new ObservableCollection<ProcessWantModel>();
        
        SkillOptions = new ObservableCollection<string>();
        TechOptions = new ObservableCollection<string>();
        ProductOptions = new ObservableCollection<string>(dc.Products.Keys);

        AddInputProduct = ReactiveCommand.Create(_addInputProduct);
        AddCapitalProduct = ReactiveCommand.Create(_addCapitalProduct);
        AddOutputProduct = ReactiveCommand.Create(_addOutputProduct);
        EditInputProduct = ReactiveCommand.Create(_editInputProduct);
        EditCapitalProduct = ReactiveCommand.Create(_editCapitalProduct);
        EditOutputProduct = ReactiveCommand.Create(_editOutputProduct);
        RemoveInputProduct = ReactiveCommand.Create(_removeInputProduct);
        RemoveCapitalProduct = ReactiveCommand.Create(_removeCapitalProduct);
        RemoveOutputProduct = ReactiveCommand.Create(_removeOutputProduct);
        
        AddInputWant = ReactiveCommand.Create(_addInputWant);
        AddCapitalWant = ReactiveCommand.Create(_addCapitalWant);
        AddOutputWant = ReactiveCommand.Create(_addOutputWant);
        EditInputWant = ReactiveCommand.Create(_editInputWant);
        EditCapitalWant = ReactiveCommand.Create(_editCapitalWant);
        EditOutputWant = ReactiveCommand.Create(_editOutputWant);
        RemoveInputWant = ReactiveCommand.Create(_removeInputWant);
        RemoveCapitalWant = ReactiveCommand.Create(_removeCapitalWant);
        RemoveOutputWant = ReactiveCommand.Create(_removeOutputWant);
        CommitProcess = ReactiveCommand.Create(_commitProcess);
    }

    public ProcessEditorViewModel(Window win) : this()
    {
        _window = win;
        
        // get all options for a new process
        foreach (var skill in dc.Skills.Keys)
            SkillOptions.Add(skill);
        foreach (var tech in dc.Technologies.Keys)
            TechOptions.Add(tech);
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

        // get all options for a new process
        foreach (var skill in dc.Skills.Keys)
            SkillOptions.Add(skill);
        foreach (var tech in dc.Technologies.Keys
                     .Where(x => x != model.TechRequirement))
            TechOptions.Add(tech);

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
        
        // add products and wants
        InputProducts = new ObservableCollection<ProcessProductModel>(model.InputProducts);
        CapitalProducts = new ObservableCollection<ProcessProductModel>(model.CapitalProducts);
        OutputProducts = new ObservableCollection<ProcessProductModel>(model.OutputProducts);
        InputWants = new ObservableCollection<ProcessWantModel>(model.InputWants);
        CapitalWants = new ObservableCollection<ProcessWantModel>(model.CapitalWants);
        OutputWants = new ObservableCollection<ProcessWantModel>(model.OutputWants);
    }

    #region PartFunctions

    public ReactiveCommand<Unit, Task> AddInputProduct { get; set; }
    public ReactiveCommand<Unit, Task> AddCapitalProduct { get; set; }
    public ReactiveCommand<Unit, Task> AddOutputProduct { get; set; }
    public ReactiveCommand<Unit, Task> EditInputProduct { get; set; }
    public ReactiveCommand<Unit, Task> EditCapitalProduct { get; set; }
    public ReactiveCommand<Unit, Task> EditOutputProduct { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveInputProduct { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveCapitalProduct { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveOutputProduct { get; set; }
    
    public ReactiveCommand<Unit, Task> AddInputWant { get; set; }
    public ReactiveCommand<Unit, Task> AddCapitalWant { get; set; }
    public ReactiveCommand<Unit, Task> AddOutputWant { get; set; }
    public ReactiveCommand<Unit, Task> EditInputWant { get; set; }
    public ReactiveCommand<Unit, Task> EditCapitalWant { get; set; }
    public ReactiveCommand<Unit, Task> EditOutputWant { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveInputWant { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveCapitalWant { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveOutputWant { get; set; }
    
    private async Task _addInputProduct() => await _addProduct(ProcessPartTag.Input);
    private async Task _addCapitalProduct() => await _addProduct(ProcessPartTag.Capital);
    private async Task _addOutputProduct() => await _addProduct(ProcessPartTag.Output);
    private async Task _editInputProduct() => await _editProduct(SelectedInputProduct, ProcessPartTag.Input);
    private async Task _editCapitalProduct() => await _editProduct(SelectedCapitalProduct, ProcessPartTag.Capital);
    private async Task _editOutputProduct() => await _editProduct(SelectedOutputProduct, ProcessPartTag.Output);
    private void _removeInputProduct() => _removeProduct(SelectedInputProduct, ProcessPartTag.Input);
    private void _removeCapitalProduct() => _removeProduct(SelectedCapitalProduct, ProcessPartTag.Capital);
    private void _removeOutputProduct() => _removeProduct(SelectedOutputProduct, ProcessPartTag.Output);

    private async Task _addInputWant() => await _addWant(ProcessPartTag.Input);
    private async Task _addCapitalWant() => await _addWant(ProcessPartTag.Capital);
    private async Task _addOutputWant() => await _addWant(ProcessPartTag.Output);
    private async Task _editInputWant() => await _editWant(SelectedInputWant, ProcessPartTag.Input);
    private async Task _editCapitalWant() => await _editWant(SelectedCapitalWant, ProcessPartTag.Capital);
    private async Task _editOutputWant() => await _editWant(SelectedOutputWant, ProcessPartTag.Output);
    private void _removeInputWant() => _removeWant(SelectedInputWant, ProcessPartTag.Input);
    private void _removeCapitalWant() => _removeWant(SelectedCapitalWant, ProcessPartTag.Capital);
    private void _removeOutputWant() => _removeWant(SelectedOutputWant, ProcessPartTag.Output);

    private async Task _addProduct(ProcessPartTag part)
    {
        var win = new ProcessProductEditorWindow(part);
        await win.ShowDialog(_window);

        if (win.vm.CompleteModel == null) return;

        var newPart = win.vm.CompleteModel;
        switch (part)
        {
            case ProcessPartTag.Input:
                InputProducts.Add(newPart);
                break;
            case ProcessPartTag.Capital:
                CapitalProducts.Add(newPart);
                break;
            case ProcessPartTag.Output:
                OutputProducts.Add(newPart);
                break;
        }
    }
    
    private async Task _editProduct(ProcessProductModel? selection, ProcessPartTag part)
    {
        if (selection == null) return;
        
        var win = new ProcessProductEditorWindow(selection);
        await win.ShowDialog(_window);

        if (win.vm.CompleteModel == null) return;

        var newPart = win.vm.CompleteModel;
        switch (part)
        {
            case ProcessPartTag.Input:
                InputProducts.Remove(selection);
                InputProducts.Add(newPart);
                break;
            case ProcessPartTag.Capital:
                CapitalProducts.Remove(selection);
                CapitalProducts.Add(newPart);
                break;
            case ProcessPartTag.Output:
                OutputProducts.Remove(selection);
                OutputProducts.Add(newPart);
                break;
        }
    }

    private void _removeProduct(ProcessProductModel? selection, ProcessPartTag part)
    {
        if (selection == null) return;

        switch (part)
        {
            case ProcessPartTag.Input:
                InputProducts.Remove(selection);
                break;
            case ProcessPartTag.Capital:
                CapitalProducts.Remove(selection);
                break;
            case ProcessPartTag.Output:
                OutputProducts.Remove(selection);
                break;
        }
    }

    private async Task _addWant(ProcessPartTag part)
    {
        var win = new ProcessWantEditorWindow(part);
        await win.ShowDialog(_window);

        if (win.vm.CompleteModel == null) return;

        var newPart = win.vm.CompleteModel;
        switch (part)
        {
            case ProcessPartTag.Input:
                InputWants.Add(newPart);
                break;
            case ProcessPartTag.Capital:
                CapitalWants.Add(newPart);
                break;
            case ProcessPartTag.Output:
                OutputWants.Add(newPart);
                break;
        }
    }

    private async Task _editWant(ProcessWantModel? selection, ProcessPartTag part)
    {
        if (selection == null) return;
        
        var win = new ProcessWantEditorWindow(selection);
        await win.ShowDialog(_window);

        if (win.vm.CompleteModel == null) return;

        var newPart = win.vm.CompleteModel;
        switch (part)
        {
            case ProcessPartTag.Input:
                InputWants.Remove(selection);
                InputWants.Add(newPart);
                break;
            case ProcessPartTag.Capital:
                CapitalWants.Remove(selection);
                CapitalWants.Add(newPart);
                break;
            case ProcessPartTag.Output:
                OutputWants.Remove(selection);
                OutputWants.Add(newPart);
                break;
        }
    }

    private void _removeWant(ProcessWantModel? selection, ProcessPartTag part)
    {
        if (selection == null) return;

        switch (part)
        {
            case ProcessPartTag.Input:
                InputWants.Remove(selection);
                break;
            case ProcessPartTag.Capital:
                CapitalWants.Remove(selection);
                break;
            case ProcessPartTag.Output:
                OutputWants.Remove(selection);
                break;
        }
    }
    
    #endregion

    public ReactiveCommand<Unit, Task> CommitProcess { get; set; }

    private async Task _commitProcess()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Must have a name.");
        if (MinimumTime < 0)
            errors.Add("Minimum Time cannot be Negative Value.");
        if (SkillMin < 0 || SkillMax < 0)
            errors.Add("Skill Limits must be greater than 0.");
        // Skill min must be below max, unless there is no skill selected.
        if (SkillMin >= SkillMax && !string.IsNullOrWhiteSpace(Skill))
            errors.Add("Skill Minimum must be less than skill max.");
        // Skills aren't needed if the Process is Failure, Maintenance, Consumption, or Use
        if (string.IsNullOrWhiteSpace(Skill) && !(Failure || Maintenance || Consumption || Use))
            errors.Add("No Skill Selected");
        // check for duplicate processes
        var newCombo = string.IsNullOrWhiteSpace(VariantName) ? Name : $"{Name}({VariantName})";
        var oldCombo = string.IsNullOrWhiteSpace(_original.VariantName) ? _original.Name : $"{_original.Name}({_original.VariantName})";

        if (dc.Processes.ContainsKey(newCombo) && newCombo != oldCombo)
            errors.Add("Process is a duplicate of an existing product.");
        // ensure that if it's a FMUC process, ensure it has it's product in it.
        if (Failure || Maintenance || Consumption || Use)
        {
            // if FM or C, require it in input
            if ((Failure || Maintenance || Consumption) && InputProducts.All(x => x.Product != SelectedProduct))
            {
                errors.Add($"Process requires the product '{SelectedProduct}' as an input.");
            }
            // if U require in Capital.
            if (Use && CapitalProducts.All(x => x.Product != SelectedProduct))
            {
                errors.Add($"Use process requires product '{SelectedProduct}' as a Capital.");
            }
            // if M require it in the output
            if (Maintenance && OutputProducts.All(x => x.Product != SelectedProduct))
            {
                errors.Add($"Process requires the product '{SelectedProduct}' as an input.");
            }
        }
        
        if (errors.Any())
        {
            await MessageBoxManager
                .GetMessageBoxStandardWindow("Error!", "Errors Found:\n" + String.Join('\n', errors))
                .ShowDialog(_window);
            return;
        }
        // check for duplicates and ask if they wish to continue.
        if (InputProducts.Select(x => x.Product).Distinct().Count() != InputProducts.Count())
            errors.Add("Duplicate Input Products.");
        if (InputWants.Select(x => x.Want).Distinct().Count() != InputWants.Count())
            errors.Add("Duplicate Input Wants.");
        if (CapitalProducts.Select(x => x.Product).Distinct().Count() != CapitalProducts.Count())
            errors.Add("Duplicate Capital Products.");
        if (CapitalWants.Select(x => x.Want).Distinct().Count() != CapitalWants.Count())
            errors.Add("Duplicate Capital Wants.");
        if (OutputProducts.Select(x => x.Product).Distinct().Count() != OutputProducts.Count())
            errors.Add("Duplicate Output Products.");
        if (OutputWants.Select(x => x.Want).Distinct().Count() != OutputWants.Count())
            errors.Add("Duplicate Output Wants.");

        if (errors.Any())
        {
            var result = await MessageBoxManager
                .GetMessageBoxStandardWindow("Duplicates found!", "Duplicate Parts found. Accept anyway?", ButtonEnum.YesNo)
                .ShowDialog(_window);
            if (result == ButtonResult.No)
                return;
        }
        // TODO Add more checks on Parts here. The worst possible errors are handled by the part windows
        var oldProc = _original.Name.Any() ? dc.Processes[oldCombo] : null;
        
        // update
        if (oldProc != null)
        {
            oldProc.Name = Name;
            oldProc.VariantName = VariantName;
            oldProc.Description = Description;
            oldProc.MinimumTime = MinimumTime;
            if (!string.IsNullOrWhiteSpace(Skill))
            {
                oldProc.Skill = dc.Skills[Skill];
                oldProc.SkillMinimum = SkillMin;
                oldProc.SkillMaximum = SkillMax;
            }

            if (!string.IsNullOrWhiteSpace(TechRequirements))
                oldProc.TechRequirement = dc.Technologies[TechRequirements];
            else
                oldProc.TechRequirement = null;
            
            // check find old product connections and remove
            foreach (var product in oldProc.InputProducts)
            {
                product.Product.ProductProcesses.Remove(oldProc);
            }
            foreach (var product in oldProc.CapitalProducts)
            {
                product.Product.ProductProcesses.Remove(oldProc);
            }
            foreach (var product in oldProc.OutputProducts)
            {
                product.Product.ProductProcesses.Remove(oldProc);
            }
            // tags
            oldProc.ProcessTags.Clear();
            if (Failure)
                oldProc.ProcessTags.Add(ProcessTag.Failure);
            if (Consumption)
                oldProc.ProcessTags.Add(ProcessTag.Consumption);
            if (Maintenance)
                oldProc.ProcessTags.Add(ProcessTag.Maintenance);
            if (Use)
                oldProc.ProcessTags.Add(ProcessTag.Use);
            if (Chance)
                oldProc.ProcessTags.Add(ProcessTag.Chance);
            if (Crop)
                oldProc.ProcessTags.Add(ProcessTag.Crop);
            if (Mine)
                oldProc.ProcessTags.Add(ProcessTag.Mine);
            if (Extractor)
                oldProc.ProcessTags.Add(ProcessTag.Extractor);
            if (Tap)
                oldProc.ProcessTags.Add(ProcessTag.Tap);
            if (Refiner)
                oldProc.ProcessTags.Add(ProcessTag.Refiner);
            if (Sorter)
                oldProc.ProcessTags.Add(ProcessTag.Sorter);
            if (Scrubber)
                oldProc.ProcessTags.Add(ProcessTag.Scrubber);
            if (Scrapping)
                oldProc.ProcessTags.Add(ProcessTag.Scrapping);
            
            // parts
            oldProc.ProcessProducts.Clear();
            oldProc.ProcessWants.Clear();
            foreach (var input in InputProducts)
            {
                var newPart = new ProcessProduct
                {
                    Product = dc.Products[input.Product],
                    Amount = input.Amount,
                    Part = input.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        input.Tags)
                };
                oldProc.ProcessProducts.Add(newPart);
                dc.Products[input.Product].ProductProcesses.Add(oldProc);
            }
            foreach (var input in InputWants)
            {
                oldProc.ProcessWants.Add(new ProcessWant
                {
                    Want = dc.Wants[input.Want],
                    Amount = input.Amount,
                    Part = input.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        input.Tags)
                });
            }
            foreach (var capital in CapitalProducts)
            {
                oldProc.ProcessProducts.Add(new ProcessProduct
                {
                    Product = dc.Products[capital.Product],
                    Amount = capital.Amount,
                    Part = capital.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        capital.Tags)
                });
                if (!dc.Products[capital.Product].ProductProcesses.Contains(oldProc))
                    dc.Products[capital.Product].ProductProcesses.Add(oldProc);
            }
            foreach (var capital in CapitalWants)
            {
                oldProc.ProcessWants.Add(new ProcessWant
                {
                    Want = dc.Wants[capital.Want],
                    Amount = capital.Amount,
                    Part = capital.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        capital.Tags)
                });
            }
            foreach (var output in OutputProducts)
            {
                oldProc.ProcessProducts.Add(new ProcessProduct
                {
                    Product = dc.Products[output.Product],
                    Amount = output.Amount,
                    Part = output.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        output.Tags)
                });
                if (!dc.Products[output.Product].ProductProcesses.Contains(oldProc))
                    dc.Products[output.Product].ProductProcesses.Add(oldProc);
            }
            foreach (var output in OutputWants)
            {
                oldProc.ProcessWants.Add(new ProcessWant
                {
                    Want = dc.Wants[output.Want],
                    Amount = output.Amount,
                    Part = output.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        output.Tags)
                });
            }
            
            // update original if name is different
            if (oldCombo != oldProc.GetName())
            {
                dc.Processes.Remove(oldCombo);
                dc.Processes[oldProc.GetName()] = oldProc;
            }

            _original = new ProcessModel(oldProc);
        }
        else
        {
            var newProcess = new Process
            {
                Name = Name,
                VariantName = VariantName,
                Description = Description,
                MinimumTime = MinimumTime,
            };
            if (!string.IsNullOrWhiteSpace(Skill))
            {
                newProcess.Skill = dc.Skills[Skill];
                newProcess.SkillMinimum = SkillMin;
                newProcess.SkillMaximum = SkillMax;
            }

            // tech requirement
            if (!string.IsNullOrWhiteSpace(TechRequirements))
                newProcess.TechRequirement = dc.Technologies[TechRequirements];
            
            // parts
            foreach (var input in InputProducts)
            {
                newProcess.ProcessProducts.Add(new ProcessProduct
                {
                    Product = dc.Products[input.Product],
                    Amount = input.Amount,
                    Part = input.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        input.Tags)
                });
                // connect to any products which it uses.
                dc.Products[input.Product].ProductProcesses.Add(newProcess);
            }
            foreach (var input in InputWants)
            {
                newProcess.ProcessWants.Add(new ProcessWant
                {
                    Want = dc.Wants[input.Want],
                    Amount = input.Amount,
                    Part = input.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        input.Tags)
                });
            }
            foreach (var capital in CapitalProducts)
            {
                newProcess.ProcessProducts.Add(new ProcessProduct
                {
                    Product = dc.Products[capital.Product],
                    Amount = capital.Amount,
                    Part = capital.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        capital.Tags)
                });
                // connect to any products which it uses.
                if (!dc.Products[capital.Product].ProductProcesses.Contains(newProcess))
                    dc.Products[capital.Product].ProductProcesses.Add(newProcess);
            }
            foreach (var capital in CapitalWants)
            {
                newProcess.ProcessWants.Add(new ProcessWant
                {
                    Want = dc.Wants[capital.Want],
                    Amount = capital.Amount,
                    Part = capital.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        capital.Tags)
                });
            }
            foreach (var output in OutputProducts)
            {
                newProcess.ProcessProducts.Add(new ProcessProduct
                {
                    Product = dc.Products[output.Product],
                    Amount = output.Amount,
                    Part = output.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        output.Tags)
                });
                // connect to any products which it uses.
                if (!dc.Products[output.Product].ProductProcesses.Contains(newProcess))
                    dc.Products[output.Product].ProductProcesses.Add(newProcess);
            }
            foreach (var output in OutputWants)
            {
                newProcess.ProcessWants.Add(new ProcessWant
                {
                    Want = dc.Wants[output.Want],
                    Amount = output.Amount,
                    Part = output.Part,
                    TagData = new List<(ProductionTag tag, Dictionary<string, object> parameters)>(
                        output.Tags)
                });
            }
            
            // tags
            if (Failure)
                newProcess.ProcessTags.Add(ProcessTag.Failure);
            if (Consumption)
                newProcess.ProcessTags.Add(ProcessTag.Consumption);
            if (Maintenance)
                newProcess.ProcessTags.Add(ProcessTag.Maintenance);
            if (Use)
                newProcess.ProcessTags.Add(ProcessTag.Use);
            if (Chance)
                newProcess.ProcessTags.Add(ProcessTag.Chance);
            if (Crop)
                newProcess.ProcessTags.Add(ProcessTag.Crop);
            if (Mine)
                newProcess.ProcessTags.Add(ProcessTag.Mine);
            if (Extractor)
                newProcess.ProcessTags.Add(ProcessTag.Extractor);
            if (Tap)
                newProcess.ProcessTags.Add(ProcessTag.Tap);
            if (Refiner)
                newProcess.ProcessTags.Add(ProcessTag.Refiner);
            if (Sorter)
                newProcess.ProcessTags.Add(ProcessTag.Sorter);
            if (Scrubber)
                newProcess.ProcessTags.Add(ProcessTag.Scrubber);
            if (Scrapping)
                newProcess.ProcessTags.Add(ProcessTag.Scrapping);

            dc.Processes.Add(newProcess.GetName(), newProcess);
            
            _original = new ProcessModel(newProcess);
        }
        
        // completed
        var success = MessageBoxManager.GetMessageBoxStandardWindow("Process Committed!",
            "Process has been committed, be sure to save the data to file!");
        await success.ShowDialog(_window);
    }

    private void UpdateEnabledTags()
    {
        if (!_updatingEnabledTags)
        {
            _updatingEnabledTags = true;
            InputsEnabled = !Failure;
            CapitalsEnabled = !Failure;

            FailureEnabled = !(Maintenance || Use || Consumption);
            ConsumptionEnabled = !(Failure || Use || Maintenance);
            MaintenanceEnabled = !(Failure || Consumption || Use);
            UseEnabled = !(Failure || Consumption || Maintenance);
            ChanceEnabled = true; // chance should always be available. 
            CropEnabled = !(Mine || Extractor || Tap || Refiner || Sorter || Scrapping || Scrubber);
            MineEnabled = !(Crop || Extractor || Tap || Refiner || Sorter || Scrapping || Scrubber);
            ExtractorEnabled = !(Mine || Crop || Tap || Refiner || Sorter || Scrapping || Scrubber);
            TapEnabled = !(Mine || Extractor || Crop || Refiner || Sorter || Scrapping || Scrubber);
            RefinerEnabled = !(Mine || Extractor || Tap || Crop || Sorter || Scrapping || Scrubber);
            SorterEnabled = !(Mine || Extractor || Tap || Refiner || Crop || Scrapping || Scrubber);
            ScrappingEnabled = !(Mine || Extractor || Tap || Refiner || Sorter || Crop || Scrubber);
            ScrubberEnabled = !(Mine || Extractor || Tap || Refiner || Sorter || Scrapping || Crop);
            _updatingEnabledTags = false;
        }
    }

    private void UpdateSelectedProduct()
    {
        if (Failure || Consumption || Maintenance || Use)
        { // if any of these are active
            // clear the parts
            InputProducts.Clear();
            InputWants.Clear();
            CapitalProducts.Clear();
            CapitalWants.Clear();
            OutputProducts.Clear();
            OutputWants.Clear();

            if (!string.IsNullOrEmpty(SelectedProduct))
            { // if a product is already selected, update it.
                if (Failure || Consumption || Maintenance)
                { // if not a use, it should be an input
                    InputProducts.Add(new ProcessProductModel
                    {
                        Product = SelectedProduct,
                        Amount = 1,
                        Part = ProcessPartTag.Input
                    });
                    
                    if (Maintenance)
                    {// if it's a maintenance also add it to the output.
                        OutputProducts.Add(new ProcessProductModel
                        {
                            Product = SelectedProduct,
                            Amount = 1,
                            Part = ProcessPartTag.Output
                        });
                    }
                }
                else if (Use) // if a use
                {
                    CapitalProducts.Add(new ProcessProductModel
                    {
                        Product = SelectedProduct,
                        Amount = 1,
                        Part = ProcessPartTag.Capital
                    });
                }
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

    public ProcessProductModel? SelectedInputProduct
    {
        get => _selectedInputProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedInputProduct, value);
    }
    
    public ProcessProductModel? SelectedCapitalProduct
    {
        get => _selectedCapitalProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedCapitalProduct, value);
    }
    
    public ProcessProductModel? SelectedOutputProduct
    {
        get => _selectedOutputProduct;
        set => this.RaiseAndSetIfChanged(ref _selectedOutputProduct, value);
    }

    public ProcessWantModel? SelectedInputWant
    {
        get => _selectedInputWant;
        set => this.RaiseAndSetIfChanged(ref _selectedInputWant, value);
    }

    public ProcessWantModel? SelectedCapitalWant
    {
        get => _selectedCapitalWant;
        set => this.RaiseAndSetIfChanged(ref _selectedCapitalWant, value);
    }

    public ProcessWantModel? SelectedOutputWant
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

    public ObservableCollection<string> SkillOptions { get; set; }
    public ObservableCollection<string> TechOptions { get; set; }
    
    public ObservableCollection<string> ProductOptions { get; set; }
}