using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Processes;
using MessageBox.Avalonia;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class JobEditorViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    private JobModel _original;
    private string _name = "";
    private string _variantName = "";
    private string _labor = "";
    private string _skill = "";
    private string _processToAdd = "";
    private string _processToRemove = "";
    private bool _canRemoveProcess;
    private bool _lockLaborAndSkill;
    private bool _laborEnabled = true;

    public JobEditorViewModel()
    {
        _original = new JobModel();
        Processes = new ObservableCollection<string>();
        LaborOptions = new ObservableCollection<string>();
        SkillOptions = new ObservableCollection<string>();
        ProcessOptions = new ObservableCollection<string>();

        AddProcess = ReactiveCommand.Create(_addProcess);
        RemoveProcess = ReactiveCommand.Create(_removeProcess);
        Commit = ReactiveCommand.Create(_commit);
    }

    public JobEditorViewModel(Window win) : this()
    {
        _window = win;
        _original = new JobModel();
        ProcessOptions = new ObservableCollection<string>(dc.Processes.Keys);
        SkillOptions = new ObservableCollection<string>(dc.Skills.Keys);
        LaborOptions = new ObservableCollection<string>(dc.Skills
            .Values.Select(x => x.Labor.GetName()));
    }

    public JobEditorViewModel(JobModel original, Window win) : this()
    {
        _window = win;
        _original = original;

        Name = original.Name;
        VariantName = original.VariantName;
        Labor = original.Labor;
        Skill = original.Skill;
        Processes = new ObservableCollection<string>(original.Processes);
        
        ProcessOptions = new ObservableCollection<string>(dc.Processes
            .Keys.Where(x => !original.Processes.Contains(x)));
        SkillOptions = new ObservableCollection<string>(dc.Skills
            .Keys);
        LaborOptions = new ObservableCollection<string>(dc.Skills
            .Values.Select(x => x.Labor.GetName()));
    }
    
    public ReactiveCommand<Unit, Unit> AddProcess { get; set; }
    public ReactiveCommand<Unit, Unit> RemoveProcess { get; set; }
    public ReactiveCommand<Unit, Task> Commit { get; set; }

    private void _addProcess()
    {
        if (!string.IsNullOrWhiteSpace(ProcessToAdd))
        {
            Processes.Add(_processToAdd);
            ProcessOptions.Remove(ProcessToAdd);
        }
    }

    private void _removeProcess()
    {
        if (!string.IsNullOrWhiteSpace(ProcessToRemove))
        {
            ProcessOptions.Add(ProcessToRemove);
            Processes.Remove(ProcessToRemove);
        }
    }

    private async Task _commit()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("Must have Name.");
        if (string.IsNullOrWhiteSpace(Labor))
            errors.Add("Must Have a Labor.");
        if (string.IsNullOrWhiteSpace(Skill))
            errors.Add("Must have a Skill");
        if (!Processes.Any())
            errors.Add("Must have at least one Process.");
        var newJob = new Job
        {
            Name = Name,
            VariantName = VariantName
        };
        if (dc.Jobs.ContainsKey(newJob.GetName()) && _original.GetName() != newJob.GetName())
            errors.Add("Job is a duplicate");

        if (errors.Any())
        {
            var error
                = MessageBoxManager.GetMessageBoxStandardWindow("Errors Found.",
                    "Errors Found:\n" + string.Join('\n', errors));
            await error.ShowDialog(_window);
            return;
        }

        Job? oldJob = null;
        if (dc.Jobs.ContainsKey(_original.GetName()))
            oldJob = dc.Jobs[_original.GetName()];
        
        // update
        if (oldJob != null)
        {
            oldJob.Name = Name;
            oldJob.VariantName = VariantName;
            oldJob.Labor = dc.Products[Labor];
            oldJob.Skill = dc.Skills[Skill];
            oldJob.Processes = new List<Process>();
            foreach (var process in Processes)
                oldJob.Processes.Add(dc.Processes[process]);
            
            // update if name has changed
            if (oldJob.GetName() != _original.GetName())
            {
                dc.Jobs.Remove(_original.GetName());
                dc.Jobs.Add(oldJob.GetName(), oldJob);
            }
            
            // update original
            _original = new JobModel(oldJob);
        }
        else
        {
            newJob.Labor = dc.Products[Labor];
            newJob.Skill = dc.Skills[Skill];
            newJob.Processes = new List<Process>();
            foreach (var process in Processes)
                newJob.Processes.Add(dc.Processes[process]);
            
            dc.Jobs.Add(newJob.GetName(), newJob);

            _original = new JobModel(newJob);
        }
        
        // done.
        var done = MessageBoxManager.GetMessageBoxStandardWindow("Success!",
            "Job has been committed. Remember to save.");
        await done.ShowDialog(_window);
    }

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

    public string Labor
    {
        get => _labor;
        set => this.RaiseAndSetIfChanged(ref _labor, value);
    }

    public string Skill
    {
        get => _skill;
        set
        {
            this.RaiseAndSetIfChanged(ref _skill, value);
            if (LockLaborAndSkill)
            {
                var labor = dc.Skills[_skill].Labor.GetName();
                Labor = labor;
            }
        }
    }

    public ObservableCollection<string> Processes { get; set; }

    public string ProcessToAdd
    {
        get => _processToAdd;
        set => this.RaiseAndSetIfChanged(ref _processToAdd, value);
    }

    public string ProcessToRemove
    {
        get => _processToRemove;
        set
        {
            this.RaiseAndSetIfChanged(ref _processToRemove, value);
            CanRemoveProcess = !string.IsNullOrWhiteSpace(_processToRemove);
        }
    }

    public bool CanRemoveProcess
    {
        get => _canRemoveProcess;
        set => this.RaiseAndSetIfChanged(ref _canRemoveProcess, value);
    }

    public bool LockLaborAndSkill
    {
        get => _lockLaborAndSkill;
        set
        {
            this.RaiseAndSetIfChanged(ref _lockLaborAndSkill, value);
            LaborEnabled = !_lockLaborAndSkill;
        }
    }

    public bool LaborEnabled
    {
        get => _laborEnabled;
        set => this.RaiseAndSetIfChanged(ref _laborEnabled, value);
    }

    public ObservableCollection<string> LaborOptions { get; set; }
    public ObservableCollection<string> SkillOptions { get; set; }
    public ObservableCollection<string> ProcessOptions { get; set; }
}