using System.Collections.ObjectModel;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using EconomicSim.Objects;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class SkillGroupEditorViewModel : ViewModelBase
{
    private string _name = "";
    private string _description = "";
    private decimal _default;
    private SkillGroupEditorWindow? window;
    private IDataContext dc = DataContextFactory.GetDataContext;
    private SkillGroupEditorModel orignial;

    public SkillGroupEditorViewModel()
    {
        orignial = new SkillGroupEditorModel();
        window = null;
        Skills = new ObservableCollection<string>();
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
    }
    
    public SkillGroupEditorViewModel(SkillGroupEditorWindow window)
    {
        orignial = new SkillGroupEditorModel();
        this.window = window;
        Skills = new ObservableCollection<string>();
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
    }
    
    public SkillGroupEditorViewModel(SkillGroupEditorWindow window, SkillGroupEditorModel group)
    {
        orignial = group;
        this.window = window;
        Skills = new ObservableCollection<string>();
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
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

    public ObservableCollection<string> Skills { get; set; }
    public ObservableCollection<string> SkillOptions { get; set; }
}