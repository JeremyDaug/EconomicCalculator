using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class FirmListViewModel
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private readonly Window _window;

    public FirmListViewModel()
    {
        Firms = new ObservableCollection<FirmModel>();
        foreach (var firm in dc.Firms.Values)
            Firms.Add(new FirmModel(firm));
    }

    public FirmListViewModel(Window win) : this()
    {
        _window = win;
    }

    private async Task _addFirm()
    {
        var newFirm = new FirmEditorWindow();
        await newFirm.ShowDialog(_window);
        
        // refresh firms
        Firms.Clear();
        foreach (var firm in dc.Firms.Values)
            Firms.Add(new FirmModel(firm));
    }

    private async Task _editFirm()
    {
        if (SelectedFirm == null) return;
        
        var newFirm = new FirmEditorWindow(SelectedFirm);
        await newFirm.ShowDialog(_window);
        
        // refresh firms
        Firms.Clear();
        foreach (var firm in dc.Firms.Values)
            Firms.Add(new FirmModel(firm));
    }

    private async Task _saveFirm()
    {
        dc.SaveFirms(dc.CurrentSave);
    }
    
    public ObservableCollection<FirmModel> Firms { get; set; }
    
    public FirmModel? SelectedFirm { get; set; }
    
    public ReactiveCommand<Unit, Task> AddFirm { get; set; }
    public ReactiveCommand<Unit, Task> EditFirm { get; set; }
    public ReactiveCommand<Unit, Task> SaveFirms { get; set; }
}