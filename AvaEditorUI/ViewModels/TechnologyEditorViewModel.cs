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
using EconomicSim.Objects.Technology;
using MessageBox.Avalonia;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class TechnologyEditorViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    private TechnologyEditorModel original;
    private string _name = "";
    private string _description = "";
    private string _category = TechCategory.Primary.ToString();
    private int _tier;
    private int _techCostBase;
    private string? _familyToAdd;
    private string? _familyToRemove;
    private string? _parentToAdd;
    private string? _parentToRemove;
    private string? _childToAdd;
    private string? _childToRemove;

    public TechnologyEditorViewModel()
    {
        _window = null;
        original = new TechnologyEditorModel();
        Families = new ObservableCollection<string>();
        Parents = new ObservableCollection<string>();
        Children = new ObservableCollection<string>();
        PossibleParents = new ObservableCollection<string>(dc.Technologies.Values
            .Where(x => x.Tier <= Tier).Select(x => x.Name));
        PossibleChildren = new ObservableCollection<string>(dc.Technologies.Values
            .Where(x => x.Tier >= Tier).Select(x => x.Name));
        AvailableFamilies = new ObservableCollection<string>(dc.TechFamilies.Keys);
        CategoryOptions = new ObservableCollection<string>(Enum.GetNames(typeof(TechCategory)));
        AddParent = ReactiveCommand.Create(AddParentToTech);
        RemoveParent = ReactiveCommand.Create(RemoveParentFromTech);
        AddChild = ReactiveCommand.Create(AddChildToTech);
        RemoveChild = ReactiveCommand.Create(RemoveChildFromTech);
        AddFamily = ReactiveCommand.Create(AddFamilyToTech);
        RemoveFamily = ReactiveCommand.Create(RemoveFamilyFromTech);
        Commit = ReactiveCommand.Create(commitTech);
    }

    public TechnologyEditorViewModel(TechnologyEditorWindow window) : this()
    {
        _window = window;
    }
    
    public TechnologyEditorViewModel(TechnologyEditorModel original, TechnologyEditorWindow window) : this()
    {
        this.original = original;
        _window = window;
        Name = original.Name;
        Description = original.Description;
        Category = original.Category;
        TechCostBase = original.TechBaseCost;
        Families = new ObservableCollection<string>(original.Families);
        Parents = new ObservableCollection<string>(original.Parents);
        Children = new ObservableCollection<string>(original.Children);
        PossibleParents = new ObservableCollection<string>();
        PossibleChildren = new ObservableCollection<string>();
        Tier = original.Tier;
    }
    
    public ReactiveCommand<Unit, Unit> AddParent { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveParent { get; set; }
    public ReactiveCommand<Unit, Unit> AddChild { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveChild { get; set; }
    public ReactiveCommand<Unit, Unit> AddFamily { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveFamily { get; set; }
    public ReactiveCommand<Unit, Task> Commit { get; set; }

    private async Task commitTech()
    {
        // ensure no errors
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Tech Must have a name.");
        if (dc.Technologies.ContainsKey(Name) && original.Name != Name)
            errors.Add("Tech Collides with Existing Tech Name.");
        TechCategory category;
        if (!Enum.TryParse(Category, out category))
            errors.Add("Error, you somehow screwed up the Category.");
        if (Tier < 0)
            errors.Add("Tier must be greater than 0.");
        if (TechCostBase <= 0)
            errors.Add("Tech Base Cost must be Greater than 0.");
        
        // Parents, children, and Families should be self-sanitizing, no checks needed there.
        if (errors.Any())
        {
            var errorWindow = MessageBoxManager
                .GetMessageBoxStandardWindow("Invalid Tech",
                    "Errors Found:\n" + string.Join('\n', errors));
            await errorWindow.ShowDialog(_window);
            return;
        }
        
        // make new Tech
        var newTech = new Technology
        {
            Name = Name,
            Description = Description,
            Category = category,
            Tier = Tier,
            TechCostBase = TechCostBase
        };
        
        // connect parents
        foreach (var parent in Parents)
        {
            var par = dc.Technologies[parent];
            newTech.Parents.Add(par);
            par.Children.Add(newTech);
        }
        // connect children
        foreach (var child in Children)
        {
            var chi = dc.Technologies[child];
            newTech.Children.Add(chi);
            chi.Parents.Add(newTech);
        }
        // connect families
        foreach (var fam in Families)
        {
            var family = dc.TechFamilies[fam];
            newTech.Families.Add(family);
            family.Techs.Add(newTech);
        }
        // update or add to our data
        if (dc.Technologies.ContainsKey(original.Name))
            dc.Technologies.Remove(original.Name);
        dc.Technologies.Add(newTech.Name, newTech);
        // update original for potential followup changes.
        original = new TechnologyEditorModel
        {
            Name = Name,
            Description = Description,
            Category = Category,
            TechBaseCost = TechCostBase,
            Tier = Tier,
            Children = new ObservableCollection<string>(Children),
            Families = new ObservableCollection<string>(Families),
            Parents = new ObservableCollection<string>(Parents)
        };
        
        var success = MessageBoxManager
            .GetMessageBoxStandardWindow("Tech Committed",
                "Errors Found:\n" + string.Join('\n', errors));
        await success.ShowDialog(_window);
    }

    private void AddParentToTech()
    {
        if (ParentToAdd == null) return;
        Parents.Add(ParentToAdd);
        PossibleParents.Remove(ParentToAdd);
    }

    private void RemoveParentFromTech()
    {
        if (ParentToRemove == null) return;
        PossibleParents.Add(ParentToRemove);
        Parents.Remove(ParentToRemove);
    }

    private void AddChildToTech()
    {
        if (ChildToAdd == null) return;
        Children.Add(ChildToAdd);
        PossibleChildren.Remove(ChildToAdd);
    }

    private void RemoveChildFromTech()
    {
        if (ChildToRemove == null) return;
        PossibleChildren.Add(ChildToRemove);
        Children.Remove(ChildToRemove);
    }

    private void AddFamilyToTech()
    {
        if (FamilyToAdd == null) return;
        Families.Add(FamilyToAdd);
        AvailableFamilies.Remove(FamilyToAdd);
    }

    private void RemoveFamilyFromTech()
    {
        if (FamilyToRemove == null) return;
        AvailableFamilies.Add(FamilyToRemove);
        Families.Remove(FamilyToRemove);
    }
    
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public string Category
    {
        get => _category;
        set => this.RaiseAndSetIfChanged(ref _category, value);
    }

    public int Tier
    {
        get => _tier;
        set
        {
            this.RaiseAndSetIfChanged(ref _tier, value);
            UpdatePossibleRelations();
        }
    }

    public int TechCostBase
    {
        get => _techCostBase;
        set => this.RaiseAndSetIfChanged(ref _techCostBase, value);
    }

    public string? FamilyToAdd
    {
        get => _familyToAdd;
        set => this.RaiseAndSetIfChanged(ref _familyToAdd, value);
    }
    
    public string? FamilyToRemove
    {
        get => _familyToRemove;
        set => this.RaiseAndSetIfChanged(ref _familyToRemove, value);
    }
    
    public string? ParentToAdd
    {
        get => _parentToAdd;
        set => this.RaiseAndSetIfChanged(ref _parentToAdd, value);
    }
    
    public string? ParentToRemove
    {
        get => _parentToRemove;
        set => this.RaiseAndSetIfChanged(ref _parentToRemove, value);
    }
    
    public string? ChildToAdd
    {
        get => _childToAdd;
        set => this.RaiseAndSetIfChanged(ref _childToAdd, value);
    }
    
    public string? ChildToRemove
    {
        get => _childToRemove;
        set => this.RaiseAndSetIfChanged(ref _childToRemove, value);
    }

    public ObservableCollection<string> Families { get; }
    public ObservableCollection<string> Parents { get; }
    public ObservableCollection<string> Children { get; }
    
    public ObservableCollection<string> PossibleParents { get; }
    public ObservableCollection<string> PossibleChildren { get; }
    public ObservableCollection<string> AvailableFamilies { get; }
    public ObservableCollection<string> CategoryOptions { get; }
    
    private void UpdatePossibleRelations()
    {
        // If tier changes, remove invalid parents and children.
        var parentsToRemove = new List<string>();
        var childrenToRemove = new List<string>();
        foreach (var parent in Parents)
            if (dc.Technologies[parent].Tier > _tier) // remove parents of a higher tier
                parentsToRemove.Add(parent);
        foreach (var child in Children) // remove children of a lower tier
            if (dc.Technologies[child].Tier < _tier)
                childrenToRemove.Add(child);
        foreach (var parent in parentsToRemove)
            Parents.Remove(parent);
        foreach (var child in childrenToRemove)
            Children.Remove(child);
        // update Possible Parents and children
        PossibleChildren.Clear();
        PossibleParents.Clear();
        foreach (var child in dc.Technologies.Values
                     .Where(x => x.Tier >= _tier)
                     .Select(x => x.Name)
                     .Where(x => !Children.Contains(x)))
            PossibleChildren.Add(child);
        foreach (var parent in dc.Technologies.Values
                     .Where(x => x.Tier <= _tier)
                     .Select(x => x.Name)
                     .Where(x => !Parents.Contains(x)))
            PossibleParents.Add(parent);
    }
}