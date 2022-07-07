using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using MessageBox.Avalonia;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class ProcessListViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    private ProcessModel? _selectedProcess;


    public ProcessListViewModel()
    {
        Processes = new List<ProcessModel>();

        NewProcess = ReactiveCommand.Create(AddProcess);
        EditProcess = ReactiveCommand.Create(EditSelectedProcess);
        SaveProcesses = ReactiveCommand.Create(SaveAllProcesses);
        
        foreach (var process in dc.Processes.Values)
            Processes.Add(new ProcessModel(process));
    }

    public ProcessListViewModel(Window win) : this()
    {
        _window = win;
    }

    private async Task AddProcess()
    {
        var win = new ProcessEditorWindow();
        await win.ShowDialog(_window);
        
        Processes.Clear();
        foreach (var proc in dc.Processes.Values)
            Processes.Add(new ProcessModel(proc));
    }

    private async Task EditSelectedProcess()
    {
        if (SelectedProcess == null) return;

        var win = new ProcessEditorWindow(SelectedProcess);
        await win.ShowDialog(_window);
        
        Processes.Clear();
        foreach (var proc in dc.Processes.Values)
            Processes.Add(new ProcessModel(proc));
        SelectedProcess = null;
    }

    private async Task SaveAllProcesses()
    {
        dc.SaveProcesses();

        var success = MessageBoxManager.GetMessageBoxStandardWindow("Processes Saved!", "Processes Saved!");
        await success.ShowDialog(_window);
    }
    
    public List<ProcessModel> Processes { get; set; }
    
    public ReactiveCommand<Unit, Task> NewProcess { get; set; }
    public ReactiveCommand<Unit, Task> EditProcess { get; set; }
    public ReactiveCommand<Unit, Task> SaveProcesses { get; set; }

    public ProcessModel? SelectedProcess
    {
        get => _selectedProcess;
        set => this.RaiseAndSetIfChanged(ref _selectedProcess, value);
    }
}