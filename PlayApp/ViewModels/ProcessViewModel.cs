using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Processes;
using ReactiveUI;

namespace PlayApp.ViewModels;

public class ProcessViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window _window;
    private IProcess process;
    private ProcessModel _model;
    private string _name;
    private string _variantName;
    private string _description;
    private decimal _minimumTime;
    private string _skill;
    private decimal _skillMin;
    private decimal _skillMax;
    private string _techRequirements;

    public ProcessViewModel()
    {
        EditProcess = ReactiveCommand.Create(_editProcess);
    }

    public ProcessViewModel(IProcess process, Window window)
    {
        _model = new ProcessModel(process);
        _window = window;
        
        InputProducts = new ObservableCollection<string>(_model.InputProducts
            .Select(x => $"{x} \n{x.TagString}"));
        InputWants = new ObservableCollection<string>(_model.InputWants
            .Select(x => x.ToString() + x.TagString));
        CapitalProducts = new ObservableCollection<string>(_model.CapitalProducts
            .Select(x => x.ToString() + x.TagString));
        CapitalWants = new ObservableCollection<string>(_model.CapitalWants
            .Select(x => x.ToString() + x.TagString));
        OutputProducts = new ObservableCollection<string>(_model.OutputProducts
            .Select(x => x.ToString() + x.TagString));
        OutputWants = new ObservableCollection<string>(_model.OutputWants
            .Select(x => x.ToString() + x.TagString));

        Name = _model.Name;
        VariantName = _model.VariantName;
        Description = _model.Description;
        MinimumTime = _model.MinimumTime;
        Skill = _model.Skill;
        SkillMin = _model.SkillMin;
        SkillMax = _model.SkillMax;
        TechRequirements = _model.TechRequirement;

        DebugEnabled = dc.DebugMode;
        EditProcess = ReactiveCommand.Create(_editProcess);
    }
    
    public bool DebugEnabled { get; set; }
    
    public ObservableCollection<string> InputProducts { get; set; }
    public ObservableCollection<string> InputWants { get; set; }
    public ObservableCollection<string> CapitalProducts { get; set; }
    public ObservableCollection<string> CapitalWants { get; set; }
    public ObservableCollection<string> OutputProducts { get; set; }
    public ObservableCollection<string> OutputWants { get; set; }
    
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

    public ReactiveCommand<Unit, Task> EditProcess { get; set; }

    private async Task _editProcess()
    {
        var editor = new ProcessEditorWindow(_model);
        await editor.ShowDialog(_window);
        //RefreshProcess();
    }
}
