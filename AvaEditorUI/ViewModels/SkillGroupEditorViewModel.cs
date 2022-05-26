using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using EconomicSim.Objects;
using EconomicSim.Objects.Skills;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class SkillGroupEditorViewModel : ViewModelBase
{
    private string _name = "";
    private string _description = "";
    private decimal _default;
    private SkillGroupEditorWindow? _window;
    private IDataContext dc = DataContextFactory.GetDataContext;
    private SkillGroupEditorModel orignial;
    private string? _selectedSkill;
    private string? _skillToAdd;
    private bool _canAdd;
    private bool _canRemove;

    public SkillGroupEditorViewModel()
    {
        orignial = new SkillGroupEditorModel();
        _window = null;
        Skills = new ObservableCollection<string>();
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
        AddSkill = ReactiveCommand.Create(AddSkillToGroup);
        RemoveSkill = ReactiveCommand.Create(RemoveSkillFromGroup);
        Commit = ReactiveCommand.Create(CommitGroup);
    }
    
    public SkillGroupEditorViewModel(SkillGroupEditorWindow window)
    {
        orignial = new SkillGroupEditorModel();
        this._window = window;
        Skills = new ObservableCollection<string>();
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
        AddSkill = ReactiveCommand.Create(AddSkillToGroup);
        RemoveSkill = ReactiveCommand.Create(RemoveSkillFromGroup);
        Commit = ReactiveCommand.Create(CommitGroup);
    }
    
    public SkillGroupEditorViewModel(SkillGroupEditorWindow window, SkillGroupEditorModel group)
    {
        orignial = group;
        this._window = window;
        Name = group.Name;
        Description = group.Description;
        Default = group.Default;
        Skills = new ObservableCollection<string>(group.Skills);
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
        AddSkill = ReactiveCommand.Create(AddSkillToGroup);
        RemoveSkill = ReactiveCommand.Create(RemoveSkillFromGroup);
        Commit = ReactiveCommand.Create(CommitGroup);
    }
    
    public ReactiveCommand<Unit, Unit> AddSkill { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveSkill { get; set; }
    public ReactiveCommand<Unit, Task> Commit { get; set; }

    private async Task CommitGroup()
    {
        var errors = new List<string>();
        // check name
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Invalid Name.");
        if (dc.SkillGroups.ContainsKey(Name) && Name != orignial.Name)
            errors.Add("Group is Duplicate of existing");
        if (Default is > 1 or < 0)
            errors.Add("Default must be between 0 and 1");

        if (errors.Any())
        {
            var failure = MessageBox.Avalonia.MessageBoxManager
                .GetMessageBoxStandardWindow("Errors Found", 
                    string.Join('\n', errors));
            await failure.ShowDialog(_window);
            return;
        }
        
        // update original
        var newModel = new SkillGroupEditorModel
        {
            Name = Name,
            Description = Description,
            Default = Default, 
            Skills = new ObservableCollection<string>(Skills)
        };

        var newGroup = new SkillGroup
        {
            Name = Name,
            Default = Default,
            Description = Description
        };

        foreach (var skill in Skills)
        {// add skills to group.
            var connection = dc.Skills[skill];
            newGroup.Skills.Add(connection);
            // add group to skills
            connection.Groups.Add(newGroup);
        }

        // if the original already exists, remove to replace
        if (dc.SkillGroups.ContainsKey(orignial.Name))
            dc.SkillGroups.Remove(orignial.Name);
        dc.SkillGroups.Add(newGroup.Name, newGroup);
        orignial = newModel;
        
        // finished.
        var success = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow("Skill Group Committed!", 
                "Skill Group has been committed, be sure to save!");
        await success.ShowDialog(_window);
    }
    
    private void AddSkillToGroup()
    {
        if (SkillToAdd == null)
            return;
        
        Skills.Add(SkillToAdd);
        SkillOptions.Remove(SkillToAdd);
    }

    private void RemoveSkillFromGroup()
    {
        if (SelectedSkill == null)
            return;
        
        SkillOptions.Add(SelectedSkill);
        Skills.Remove(SelectedSkill);
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

    public decimal Default
    {
        get => _default;
        set => this.RaiseAndSetIfChanged(ref _default, value);
    }

    public string? SelectedSkill
    {
        get => _selectedSkill;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedSkill, value);
            CanRemove = value != null;
        }
    }

    public string? SkillToAdd
    {
        get => _skillToAdd;
        set
        {
            this.RaiseAndSetIfChanged(ref _skillToAdd, value);
            CanAdd = value != null;
        }
    }

    public ObservableCollection<string> Skills { get; set; }
    public ObservableCollection<string> SkillOptions { get; set; }

    public bool CanRemove
    {
        get => _canRemove;
        set => this.RaiseAndSetIfChanged(ref _canRemove, value);
    }

    public bool CanAdd
    {
        get => _canAdd;
        set => this.RaiseAndSetIfChanged(ref _canAdd, value);
    }
}