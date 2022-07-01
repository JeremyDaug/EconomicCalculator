using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Models;
using AvaEditorUI.Views;
using Avalonia.Controls;
using EconomicSim.Objects;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class SpeciesListViewModel
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    
    public SpeciesListViewModel()
    {
        NewSpecies = ReactiveCommand.Create(_newSpecies);
        EditSpecies = ReactiveCommand.Create(_editSpecies);
        SaveSpecies = ReactiveCommand.Create(_saveSpecies);

        Species = new ObservableCollection<SpeciesModel>();

        foreach (var species in dc.Species.Values)
        {
            Species.Add(new SpeciesModel(species));
        }
    }

    public SpeciesListViewModel(Window window) : this()
    {
        _window = window;
    }

    private async Task _newSpecies()
    {
        var win = new SpeciesEditorWindow();
        await win.ShowDialog(_window);
        Species.Clear();
        foreach (var species in dc.Species.Values)
            Species.Add(new SpeciesModel(species));
    }

    private async Task _editSpecies()
    {
        if (SelectedSpecies == null) return;
        
        var win = new SpeciesEditorWindow(SelectedSpecies);
        await win.ShowDialog(_window);
        Species.Clear();
        foreach (var species in dc.Species.Values)
            Species.Add(new SpeciesModel(species));
    }

    private void _saveSpecies()
    {
        dc.SaveSpecies();
    }
    
    public SpeciesModel? SelectedSpecies { get; set; }

    public ObservableCollection<SpeciesModel> Species { get; set; }
    
    public ReactiveCommand<Unit, Task> NewSpecies { get; set; }
    public ReactiveCommand<Unit, Task> EditSpecies { get; set; }
    public ReactiveCommand<Unit, Unit> SaveSpecies { get; set; }
}