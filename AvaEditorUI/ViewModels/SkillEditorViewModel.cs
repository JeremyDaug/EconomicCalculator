using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Helpers;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using EconomicSim.Objects;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Products.ProductTags;
using EconomicSim.Objects.Skills;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class SkillEditorViewModel : ViewModelBase
{
    private SkillEditorModel _original;

    private IDataContext dc = DataContextFactory.GetDataContext;
    private string _name = "";
    private string _description = "";
    private string _labor = "";
    private string _skillToAdd = "";
    private Pair<string, decimal>? _selectedRelation;
    private bool _canAddSkill;
    private bool _canRemoveSkill;
    private string? _groupToAdd;
    private bool _canAddGroup;
    private bool _canRemoveGroup;
    private Dummy<string>? _selectedGroup;

    public SkillEditorViewModel()
    {
        _original = new SkillEditorModel();

        Relations = new ObservableCollection<Pair<string, decimal>>();
        Groups = new ObservableCollection<Dummy<string>>();

        GroupOptions = new ObservableCollection<string>(dc.SkillGroups.Keys);
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
        
        // get products which are services and haven't been taken by another skills
        LaborOptions = new ObservableCollection<string>(dc.Products.Values
            .Where(x => x.ProductTags.Keys.Contains(ProductTag.Service))
            .Where(x => !dc.Skills.Values.Select(y => y.Labor).Contains(x))
            .Select(x => x.GetName()));
        
        AddSkill = ReactiveCommand.Create(AddSkillRelation);
        RemoveSkill = ReactiveCommand.Create(RemoveSkillRelation);
        AddGroup = ReactiveCommand.Create(AddSkillGroup);
        RemoveGroup = ReactiveCommand.Create(RemoveSkillGroup);
        //NewLabor = ReactiveCommand.Create(CreateNewLabor);
        CommitSkill = ReactiveCommand.Create(SaveOrUpdateSkill);
    }

    public SkillEditorViewModel(SkillEditorWindow win)
    {
        this.Window = win; 
        _original = new SkillEditorModel();
        
        Relations = new ObservableCollection<Pair<string, decimal>>();
        Groups = new ObservableCollection<Dummy<string>>();

        GroupOptions = new ObservableCollection<string>(dc.SkillGroups.Keys);
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
        // get products which are services and haven't been taken by another skills
        LaborOptions = new ObservableCollection<string>(dc.Products.Values
            .Where(x => x.ProductTags.Keys.Contains(ProductTag.Service))
            .Where(x => !dc.Skills.Values.Select(y => y.Labor).Contains(x))
            .Select(x => x.GetName()));
        
        AddSkill = ReactiveCommand.Create(AddSkillRelation);
        RemoveSkill = ReactiveCommand.Create(RemoveSkillRelation);
        AddGroup = ReactiveCommand.Create(AddSkillGroup);
        RemoveGroup = ReactiveCommand.Create(RemoveSkillGroup);
        //NewLabor = ReactiveCommand.Create(CreateNewLabor);
        CommitSkill = ReactiveCommand.Create(SaveOrUpdateSkill);
    }
    
    public SkillEditorViewModel(SkillEditorWindow win, SkillEditorModel model)
    {
        this.Window = win;
        _original = model;
        Name = model.Name;
        Description = model.Description;
        Labor = model.Labor;

        Relations = new ObservableCollection<Pair<string, decimal>>();
        Groups = new ObservableCollection<Dummy<string>>();
        
        foreach (var rel in model.Relations)
            Relations.Add(new Pair<string, decimal>(rel.skill, rel.rate));

        foreach (var group in model.Groups)
            Groups.Add(new Dummy<string>(group));
            
        GroupOptions = new ObservableCollection<string>(dc.SkillGroups.Keys);
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
        // get products which are services and haven't been taken by another skills
        LaborOptions = new ObservableCollection<string>(dc.Products.Values
            .Where(x => x.ProductTags.Keys.Contains(ProductTag.Service))
            .Where(x => !dc.Skills.Values.Select(y => y.Labor).Contains(x))
            .Select(x => x.GetName()));

        AddSkill = ReactiveCommand.Create(AddSkillRelation);
        RemoveSkill = ReactiveCommand.Create(RemoveSkillRelation);
        AddGroup = ReactiveCommand.Create(AddSkillGroup);
        RemoveGroup = ReactiveCommand.Create(RemoveSkillGroup);
        //NewLabor = ReactiveCommand.Create(CreateNewLabor);
        CommitSkill = ReactiveCommand.Create(SaveOrUpdateSkill);
    }
    
    public ReactiveCommand<Unit, Unit> AddSkill { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveSkill { get; set; }
    public ReactiveCommand<Unit, Unit> AddGroup { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveGroup { get; set; }
    //public ReactiveCommand<Unit, Unit> NewLabor { get; set; }
    public ReactiveCommand<Unit, Task> CommitSkill { get; set; }

    private async Task SaveOrUpdateSkill()
    {
        var errors = new List<string>();
        // ensure it's a proper name
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Name cannot be empty or whitespace.");
        // Should not be duplicate if it doesn't reference existing skill.
        if (dc.Skills.ContainsKey(Name) && _original.Name != Name)
            errors.Add("Name is Duplicate of existing Item.");
        // Must have labor
        if (string.IsNullOrEmpty(Labor))
            errors.Add("Must Select a Labor.");
        // check for duplicate relations.
        if (Relations.GroupBy(x => x.Primary)
            .Any(x => x.Count() > 1))
            errors.Add("Cannot contain duplicate Relations.");
        if (Relations.Any(x => x.Secondary is > 1 or < 0))
            errors.Add("Relation Rate must be between 0 and 1.");
        if (Groups.GroupBy(x => x.Wrapped)
            .Any(x => x.Count() > 1))
            errors.Add("Duplicate Groups found.");

        if (errors.Any())
        {
            var errBox = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Errors found.", 
                    string.Join('\n', errors));
            await errBox.ShowDialog(Window);
            return;
        }

        Product actualLabor;
        if (!dc.Products.ContainsKey(Labor))
        {
            actualLabor = Product.ServiceExample();
            actualLabor.Name = Labor;
            dc.Products[Labor] = actualLabor;
        }
        else
        {
            actualLabor = dc.Products[Labor];
        }

        var model = new SkillEditorModel
        {
            Name = Name,
            Description = Description,
            Labor = Labor,
            Relations = new ObservableCollection<(string skill, decimal rate)>(),
            Groups = new ObservableCollection<string>(Groups.Select(x => x.Wrapped))
        };
        
        var newSkill = new Skill
        {
            Name = Name,
            Description = Description,
            Labor = actualLabor,
        };

        foreach (var rel in Relations)
        {
            newSkill.Relations.Add((dc.Skills[rel.Primary], rel.Secondary));
            model.Relations.Add((rel.Primary, rel.Secondary));
        }

        foreach (var group in Groups)
            newSkill.Groups.Add(dc.SkillGroups[group.Wrapped]);
        
        // Add or update in context
        if (dc.Skills.ContainsKey(_original.Name))
            dc.Skills.Remove(_original.Name);
        dc.Skills[newSkill.Name] = newSkill;
        // update any groups to connect back to the new skill
        foreach (var group in Groups.Select(x => x.Wrapped))
        {
            var realGroup = dc.SkillGroups[group];
            var item = realGroup.Skills.SingleOrDefault(x => x.Name == _original.Name);
            if (item != null) // if original is in group, remove it
                realGroup.Skills.Remove(item);
            realGroup.Skills.Add(newSkill);
        }
        // update related skills
        foreach (var rel in Relations)
        {
            // TODO: ISSUE #63 Allow for a skill to define it's return rate from a skill. 
            var realSkill = dc.Skills[rel.Primary];
            var item = realSkill.Relations.SingleOrDefault(x => x.relation.Name == _original.Name);
            if (item.relation != null)
                realSkill.Relations.Remove(item);
            realSkill.Relations.Add((newSkill, rel.Secondary));
        }
        
        // update origin to update this
        _original = model;
        
        var success = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow("Skill Committed!", 
                "Remember to save Skills!");
        await success.ShowDialog(Window);
    }

    /*
    private void CreateNewLabor()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return;
        var labor = Product.ServiceExample();
        LaborOptions.Add(Name);
        labor.Name = Name;

        Labor = labor.Name;

        MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow("New Labor made.", 
                "New labor planned, will be created on save." +
                " Update or edit it from the Products screens and remember to save products.",
                ButtonEnum.Ok).Show();
    } */
    
    private void AddSkillGroup()
    {
        if (!string.IsNullOrEmpty(_groupToAdd))
        {
            Groups.Add(new Dummy<string>(_groupToAdd));
            SkillOptions.Remove(_groupToAdd);
        }
    }

    private void RemoveSkillGroup()
    {
        if (SelectedGroup != null)
            Groups.Remove(SelectedGroup);
    }
    
    private void AddSkillRelation()
    {
        if (!string.IsNullOrEmpty(_skillToAdd))
            Relations.Add(new Pair<string, decimal>(SkillToAdd, 0));
    }

    private void RemoveSkillRelation()
    {
        if (_selectedRelation != null)
            Relations.Remove(_selectedRelation);
    }
    
    public SkillEditorWindow? Window { get; set; }
    
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

    public string Labor
    {
        get => _labor;
        set => this.RaiseAndSetIfChanged(ref _labor, value);
    }

    public string SkillToAdd
    {
        get => _skillToAdd;
        set
        {
            this.RaiseAndSetIfChanged(ref _skillToAdd, value);
            CanAddSkill = !string.IsNullOrEmpty(value);
        }
    }

    public bool CanAddSkill
    {
        get => _canAddSkill;
        set => this.RaiseAndSetIfChanged(ref _canAddSkill, value);
    }

    public bool CanRemoveSkill
    {
        get => _canRemoveSkill;
        set => this.RaiseAndSetIfChanged(ref _canRemoveSkill, value);
    }

    public Pair<string, decimal>? SelectedRelation
    {
        get => _selectedRelation;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedRelation, value);
            CanRemoveSkill = value != null;
        }
    }

    public ObservableCollection<Pair<string, decimal>> Relations { get; set; }
    public ObservableCollection<Dummy<string>> Groups { get; set; }

    public ObservableCollection<string> SkillOptions { get; set; }
    public ObservableCollection<string> GroupOptions { get; set; }
    public ObservableCollection<string> LaborOptions { get; set; }

    public string? GroupToAdd
    {
        get => _groupToAdd;
        set
        {
            this.RaiseAndSetIfChanged(ref _groupToAdd, value);
            CanAddGroup = !string.IsNullOrEmpty(value);

        }
    }

    public bool CanAddGroup
    {
        get => _canAddGroup;
        set => this.RaiseAndSetIfChanged(ref _canAddGroup, value);
    }

    public bool CanRemoveGroup
    {
        get => _canRemoveGroup;
        set => this.RaiseAndSetIfChanged(ref _canRemoveGroup, value);
    }

    public Dummy<string>? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedGroup, value);
            CanRemoveGroup = value != null;
        } 
    }
}