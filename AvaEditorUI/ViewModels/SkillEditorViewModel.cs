using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
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
            .Where(x => x.ProductTags.Select(y => y.tag).Contains(ProductTag.Service))
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
            .Where(x => x.ProductTags.Select(y => y.tag).Contains(ProductTag.Service))
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
            
        GroupOptions = new ObservableCollection<string>(dc.SkillGroups.Keys
            .Where(x => !model.Groups.Contains(x)));
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
        // get products which are services and haven't been taken by another skills
        LaborOptions = new ObservableCollection<string>(dc.Products.Values
            .Where(x => x.ProductTags.Select(y => y.tag).Contains(ProductTag.Service))
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
        // check for duplicate relations.
        if (Relations.GroupBy(x => x.Primary)
            .Any(x => x.Count() > 1))
            errors.Add("Cannot contain duplicate Relations.");
        // limit relations to between 0 and 1
        if (Relations.Any(x => x.Secondary is > 1 or < 0))
            errors.Add("Relation Rate must be between 0 and 1.");
        // ensure no duplicate groups found.
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

        // add or get existing labor
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

        // if update to existing
        if (dc.Skills.ContainsKey(_original.Name))
        {
            var oldSkill = dc.Skills[_original.Name];
            // update old skill's data
            oldSkill.Name = Name;
            oldSkill.Description = Description;
            oldSkill.Labor = actualLabor;
            
            // update skill's relations
            // get old relations for safety
            var oldRelations = oldSkill.Relations.ToList();
            // loop over the old connections
            foreach (var rel in oldRelations)
            {
                // check if it is still used
                if (!Relations.Select(x => x.Primary).Contains(rel.relation.Name))
                { // if not, remove the reciprocal connection.
                    rel.relation.Relations.RemoveAt(rel.relation.Relations.FindIndex(x => x.relation == oldSkill));
                }
                // for simplicity of updating, just remove all connections.
                oldSkill.Relations.Remove(rel);
            }
            // add new relations
            foreach (var newRel in Relations)
            {
                var relSkill = dc.Skills[newRel.Primary];
                // add to our skill.
                oldSkill.Relations.Add((relSkill, newRel.Secondary));
                // if reciprocal does not already exists add it
                if (relSkill.Relations.All(x => x.relation != oldSkill)) 
                    relSkill.Relations.Add((oldSkill, newRel.Secondary));
            }
            // update skill groups
            var oldGroups = oldSkill.Groups.ToList();
            // go over each
            foreach (var group in oldGroups)
            {// check if it's still used
                if (!Groups.Select(x => x.Wrapped).Contains(group.Name))
                { // if not, remove it from it's groups.
                    group.Skills.Remove(group.Skills.Single(x => x.Name == oldSkill.Name));
                }
                // clear out old  skill connection for simplicity.
                oldSkill.Groups.Remove(group);
            }
            // re-ad connections 
            foreach (var group in Groups)
            {
                // add the group to our skill
                var currGroup = dc.SkillGroups[group.Wrapped];
                oldSkill.Groups.Add(dc.SkillGroups[currGroup.Name]);
                // if group doesn't already connect back make it
                if (currGroup.Skills.All(x => x == oldSkill))
                    currGroup.Skills.Add(oldSkill);
            }
            
            // if skill's name changed, update it's location in DataContext.
            if (_original.Name != Name)
            {
                dc.Skills.Remove(_original.Name);
                dc.Skills[Name] = oldSkill;
            }
        }
        else // if adding new skill
        {
            var newSkill = new Skill
            {
                Name = Name,
                Description = Description,
                Labor = actualLabor
            };
            // add relations and connect to the appropriate skills
            foreach (var rel in Relations)
            {
                // TODO: ISSUE #63 Allow for a skill to define it's return rate from a skill. 
                var otherSkill = dc.Skills[rel.Primary];
                newSkill.Relations.Add((otherSkill, rel.Secondary));
                otherSkill.Relations.Add((newSkill, rel.Secondary));
            }
            // add groups and reciprocal connections.
            foreach (var group in Groups)
            {
                var theGroup = dc.SkillGroups[group.Wrapped];
                newSkill.Groups.Add(theGroup);
                theGroup.Skills.Add(newSkill);
            }
            
            // add the skill to the context
            dc.Skills[newSkill.Name] = newSkill;
        }

        // update the original with our skill for future possible updates
        var ourSkill = dc.Skills[Name];
        var model = new SkillEditorModel(ourSkill);
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
        if (GroupToAdd != null)
        {
            Groups.Add(new Dummy<string>(GroupToAdd));
            GroupOptions.Remove(GroupToAdd);
        }
    }

    private void RemoveSkillGroup()
    {
        if (SelectedGroup != null)
        {
            GroupOptions.Add(SelectedGroup.Wrapped);
            Groups.Remove(SelectedGroup);
        }
    }
    
    private void AddSkillRelation()
    {
        if (!string.IsNullOrEmpty(SkillToAdd))
        {
            Relations.Add(new Pair<string, decimal>(SkillToAdd, 0));
            SkillOptions.Remove(SkillToAdd);
        }
    }

    private void RemoveSkillRelation()
    {
        if (SelectedRelation != null)
        {
            SkillOptions.Add(SelectedRelation.Primary);
            Relations.Remove(SelectedRelation);
        }
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