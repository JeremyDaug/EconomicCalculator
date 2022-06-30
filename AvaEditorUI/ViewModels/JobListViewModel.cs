using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using MessageBox.Avalonia;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class JobListViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    private JobModel? _selectedJob;

    public JobListViewModel()
    {
        Jobs = new ObservableCollection<JobModel>();
        foreach (var job in dc.Jobs.Values)
            Jobs.Add(new JobModel(job));

        AddJob = ReactiveCommand.Create(_addJob);
        EditJob = ReactiveCommand.Create(_editJob);
        SaveJobs = ReactiveCommand.Create(_saveJobs);
    }

    public JobListViewModel(Window win) : this()
    {
        _window = win;
    }

    public JobModel? SelectedJob
    {
        get => _selectedJob;
        set => this.RaiseAndSetIfChanged(ref _selectedJob, value);
    }

    public ObservableCollection<JobModel> Jobs { get; set; }
    
    public ReactiveCommand<Unit, Task> AddJob { get; set; }
    public ReactiveCommand<Unit, Task> EditJob { get; set; }
    public ReactiveCommand<Unit, Task> SaveJobs { get; set; }

    private async Task _addJob()
    {
        var win = new JobEditorWindow();
        await win.ShowDialog(_window);
        
        Jobs.Clear();
        foreach (var job in dc.Jobs.Values)
            Jobs.Add(new JobModel(job));
    }

    private async Task _editJob()
    {
        if (SelectedJob == null) return;

        var win = new JobEditorWindow(SelectedJob);
        await win.ShowDialog(_window);
        
        Jobs.Clear();
        foreach (var job in dc.Jobs.Values)
            Jobs.Add(new JobModel(job));
    }

    private async Task _saveJobs()
    {
        dc.SaveJobs();
        await MessageBoxManager.GetMessageBoxStandardWindow("Jobs Saved.", "Jobs Saved.")
            .ShowDialog(_window);
    }
}