using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Pops.Culture;
using EconomicSim.Objects.Pops.Species;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Territory;
using EconomicSim.Objects.Wants;
using MessageBox.Avalonia;
using ReactiveUI;

namespace PlayApp.ViewModels;

public class ObserverModeEntryViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window? _window;
    private string _selectedType;
    private string _selectedOption;

    public ObserverModeEntryViewModel()
    {
        TypeOptions = new ObservableCollection<string>
        {
            nameof(Territory),
            nameof(Market),
            nameof(Firm),
            nameof(PopGroup),
            nameof(Want),
            nameof(TechFamily),
            nameof(Technology),
            nameof(Product),
            nameof(Skill),
            nameof(SkillGroup),
            nameof(Process),
            nameof(Job),
            nameof(Species),
            nameof(Culture)
        };
        SelectionOptions = new ObservableCollection<string>();
    }

    public ObserverModeEntryViewModel(Window window) : this()
    {
        _window = window;
    }
    
    public ReactiveCommand<Unit, Task> View { get; set; }

    private async Task NotImplementedWindow()
    {
        var win = MessageBoxManager.GetMessageBoxStandardWindow("Not Yet Implemented.",
            "Not yet implemented, try again.");
        await win.ShowDialog(_window);
    }
    
    private async Task _view()
    {
        switch (SelectedType)
        {
            case nameof(Territory):
                await NotImplementedWindow();
                break;
            case nameof(Market):
                await NotImplementedWindow();
                break;
            case nameof(Firm):
                await NotImplementedWindow();
                break;
            case nameof(PopGroup):
                await NotImplementedWindow();
                break;
            case nameof(Want):
                await NotImplementedWindow();
                break;
            case nameof(TechFamily):
                await NotImplementedWindow();
                break;
            case nameof(Technology):
                await NotImplementedWindow();
                break;
            case nameof(Product):
                await NotImplementedWindow();
                break;
            case nameof(Skill):
                await NotImplementedWindow();
                break;
            case nameof(SkillGroup):
                await NotImplementedWindow();
                break;
            case nameof(Process):
                await NotImplementedWindow();
                break;
            case nameof(Job):
                await NotImplementedWindow();
                break;
            case nameof(Species):
                await NotImplementedWindow();
                break;
            case nameof(Culture):
                await NotImplementedWindow();
                break;
        }
    }
    
    public string SelectedType
    {
        get => _selectedType;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedType, value);
            ChangeOptions();
        }
    }

    public ObservableCollection<string> TypeOptions { get; set; }

    public string SelectedOption
    {
        get => _selectedOption;
        set => this.RaiseAndSetIfChanged(ref _selectedOption, value);
    }

    public ObservableCollection<string> SelectionOptions { get; set; }

    private void ChangeOptions()
    {
        SelectionOptions.Clear();
        
        switch (SelectedType)
        {
            case nameof(Territory):
                foreach (var option in dc.Territories.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Market):
                foreach (var option in dc.Markets.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Firm):
                foreach (var option in dc.Firms.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(PopGroup):
                foreach (var option in dc.Pops.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Want):
                foreach (var option in dc.Wants.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(TechFamily):
                foreach (var option in dc.TechFamilies.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Technology):
                foreach (var option in dc.Technologies.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Product):
                foreach (var option in dc.Products.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Skill):
                foreach (var option in dc.Skills.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(SkillGroup):
                foreach (var option in dc.SkillGroups.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Process):
                foreach (var option in dc.Processes.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Job):
                foreach (var option in dc.Jobs.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Species):
                foreach (var option in dc.Species.Keys)
                    SelectionOptions.Add(option);
                break;
            case nameof(Culture):
                foreach (var option in dc.Cultures.Keys)
                    SelectionOptions.Add(option);
                break;
        }
    }
}