using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using EconomicSim.Objects;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class SkillListsViewModel : ViewModelBase
{
    private IDataContext _dataContext;
    
    public SkillListsViewModel()
    {
        _dataContext = DataContextFactory.GetDataContext;
        NewSkill = ReactiveCommand.Create(CreateNewSkill);
        NewSkillGroup = ReactiveCommand.Create(CreateNewSkillGroup);
        EditSkill = ReactiveCommand.Create(EditSelectedSkill);
        EditSkillGroup = ReactiveCommand.Create(EditSelectedSkillGroup);
        SaveData = ReactiveCommand.Create(SaveSkillData);

        var skilllist = new List<SkillEditorModel>();
        foreach (var skill in _dataContext.Skills.Values)
            skilllist.Add(new SkillEditorModel
            {
                Name = skill.Name,
                Description = skill.Description,
                Labor = skill.Labor.GetName(),
                Groups = new ObservableCollection<string>(skill.Groups.Select(x => x.Name).ToList()),
                Relations = new ObservableCollection<(string, decimal)>(skill.Relations.Select(x => (x.relation.Name, x.rate)))
            });
        
        
        
        var grouplist = new List<SkillGroupEditorModel>();
        foreach (var group in _dataContext.SkillGroups.Values)
            grouplist.Add(new SkillGroupEditorModel
            {
                Name = group.Name,
                Description = group.Description,
                Default = group.Default,
                Skills = new ObservableCollection<string>(
                    group.Skills.Select(x => x.Name))
            });
    
        SkillsList = new ObservableCollection<SkillEditorModel>(skilllist);
        SkillGroupsList = new ObservableCollection<SkillGroupEditorModel>(grouplist);
    }
    
    public SkillListsViewModel(SkillListsWindow win)
    {
        _dataContext = DataContextFactory.GetDataContext;
        NewSkill = ReactiveCommand.Create(CreateNewSkill);
        NewSkillGroup = ReactiveCommand.Create(CreateNewSkillGroup);
        EditSkill = ReactiveCommand.Create(EditSelectedSkill);
        EditSkillGroup = ReactiveCommand.Create(EditSelectedSkillGroup);
        SaveData = ReactiveCommand.Create(SaveSkillData);

        var skilllist = new List<SkillEditorModel>();
        foreach (var skill in DataContextFactory.GetDataContext.Skills.Values)
            skilllist.Add(new SkillEditorModel
            {
                Name = skill.Name,
                Description = skill.Description,
                Labor = skill.Labor.GetName(),
                Groups = new ObservableCollection<string>(skill.Groups.Select(x => x.Name).ToList()),
                Relations = new ObservableCollection<(string, decimal)>(skill.Relations.Select(x => (x.relation.Name, x.rate)))
            });
        var grouplist = new List<SkillGroupEditorModel>();
        foreach (var group in DataContextFactory.GetDataContext.SkillGroups.Values)
            grouplist.Add(new SkillGroupEditorModel
            {
                Name = group.Name,
                Description = group.Description,
                Default = group.Default,
                Skills = new ObservableCollection<string>(
                    group.Skills.Select(x => x.Name))
            });
    
        SkillsList = new ObservableCollection<SkillEditorModel>(skilllist);
        SkillGroupsList = new ObservableCollection<SkillGroupEditorModel>(grouplist);

        Window = win;
    }

    private async Task CreateNewSkill()
    {
        var win = new SkillEditorWindow();
        await win.ShowDialog(Window);
        SkillsList.Clear();
        SkillGroupsList.Clear();
        foreach (var skill in _dataContext.Skills.Values)
            SkillsList.Add(new SkillEditorModel
            {
                Name = skill.Name,
                Description = skill.Description,
                Labor = skill.Labor.GetName(),
                Groups = new ObservableCollection<string>(skill.Groups.Select(x => x.Name).ToList()),
                Relations = new ObservableCollection<(string, decimal)>(skill.Relations.Select(x => (x.relation.Name, x.rate)))
            });
        foreach (var group in _dataContext.SkillGroups.Values)
            SkillGroupsList.Add(new SkillGroupEditorModel
            {
                Name = group.Name,
                Description = group.Description,
                Default = group.Default,
                Skills = new ObservableCollection<string>(
                    group.Skills.Select(x => x.Name))
            });
    }

    private async Task CreateNewSkillGroup()
    {
        var win = new SkillGroupEditorWindow();
        await win.ShowDialog(Window);
        SkillsList.Clear();
        SkillGroupsList.Clear();
        foreach (var skill in _dataContext.Skills.Values)
            SkillsList.Add(new SkillEditorModel
            {
                Name = skill.Name,
                Description = skill.Description,
                Labor = skill.Labor.GetName(),
                Groups = new ObservableCollection<string>(skill.Groups.Select(x => x.Name).ToList()),
                Relations = new ObservableCollection<(string, decimal)>(skill.Relations.Select(x => (x.relation.Name, x.rate)))
            });
        foreach (var group in _dataContext.SkillGroups.Values)
            SkillGroupsList.Add(new SkillGroupEditorModel
            {
                Name = group.Name,
                Description = group.Description,
                Default = group.Default,
                Skills = new ObservableCollection<string>(
                    group.Skills.Select(x => x.Name))
            });
    }

    private async Task EditSelectedSkill()
    {
        if (SelectedSkill == null)
            return;
        var win = new SkillEditorWindow(SelectedSkill);
        await win.ShowDialog(Window);
        SkillsList.Clear();
        SkillGroupsList.Clear();
        foreach (var skill in _dataContext.Skills.Values)
            SkillsList.Add(new SkillEditorModel
            {
                Name = skill.Name,
                Description = skill.Description,
                Labor = skill.Labor.GetName(),
                Groups = new ObservableCollection<string>(skill.Groups.Select(x => x.Name).ToList()),
                Relations = new ObservableCollection<(string, decimal)>(skill.Relations.Select(x => (x.relation.Name, x.rate)))
            });
        foreach (var group in _dataContext.SkillGroups.Values)
            SkillGroupsList.Add(new SkillGroupEditorModel
            {
                Name = group.Name,
                Description = group.Description,
                Default = group.Default,
                Skills = new ObservableCollection<string>(
                    group.Skills.Select(x => x.Name))
            });
    }

    private async Task EditSelectedSkillGroup()
    {
        if (SelectedSkillGroup == null)
            return;
        var win = new SkillGroupEditorWindow(SelectedSkillGroup);
        await win.ShowDialog(Window);
        SkillsList.Clear();
        SkillGroupsList.Clear();
        foreach (var skill in _dataContext.Skills.Values)
            SkillsList.Add(new SkillEditorModel
            {
                Name = skill.Name,
                Description = skill.Description,
                Labor = skill.Labor.GetName(),
                Groups = new ObservableCollection<string>(skill.Groups.Select(x => x.Name).ToList()),
                Relations = new ObservableCollection<(string, decimal)>(skill.Relations.Select(x => (x.relation.Name, x.rate)))
            });
        foreach (var group in _dataContext.SkillGroups.Values)
            SkillGroupsList.Add(new SkillGroupEditorModel
            {
                Name = group.Name,
                Description = group.Description,
                Default = group.Default,
                Skills = new ObservableCollection<string>(
                    group.Skills.Select(x => x.Name))
            });
    }

    private async Task SaveSkillData()
    {
        _dataContext.SaveSkills();
        _dataContext.SaveSkillGroups();
    }
    
    public SkillEditorModel? SelectedSkill { get; set; }
    public SkillGroupEditorModel? SelectedSkillGroup { get; set; }
    
    public ReactiveCommand<Unit, Task> NewSkill { get; }
    public ReactiveCommand<Unit, Task> NewSkillGroup { get; }
    
    public ReactiveCommand<Unit, Task> EditSkill { get; }
    public ReactiveCommand<Unit, Task> EditSkillGroup { get; }

    public ReactiveCommand<Unit, Task> SaveData { get; }

    public ObservableCollection<SkillEditorModel> SkillsList { get; set; }
    public ObservableCollection<SkillGroupEditorModel> SkillGroupsList { get; set; }
    public SkillListsWindow? Window { get; set; }
}