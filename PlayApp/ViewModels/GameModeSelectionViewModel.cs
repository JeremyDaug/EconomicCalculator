using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using PlayApp.Views;
using ReactiveUI;

namespace PlayApp.ViewModels;

public class GameModeSelectionViewModel : ViewModelBase
{
    private Window? _window;
    
    public GameModeSelectionViewModel()
    {
        ObserverMode = ReactiveCommand.Create(_observerMode);
        EntrepreneurMode = ReactiveCommand.Create(_entrepreneurMode);
        StateMode = ReactiveCommand.Create(_stateMode);
        CultureMode = ReactiveCommand.Create(_cultureMode);
        IdeoformMode = ReactiveCommand.Create(_ideoformMode);
        InstitutionMode = ReactiveCommand.Create(_instituteMode);
        MachineMode = ReactiveCommand.Create(_machineMode);
        HivemindMode = ReactiveCommand.Create(_hivemindMode);
        UndeadMode = ReactiveCommand.Create(_undeadMode);
        EcologyMode = ReactiveCommand.Create(_ecologyMode);
    }

    public GameModeSelectionViewModel(Window window) : this()
    {
        _window = window;
    }
    
    public ReactiveCommand<Unit, Task> ObserverMode { get; set; }
    public ReactiveCommand<Unit, Task> EntrepreneurMode { get; set; }
    public ReactiveCommand<Unit, Task> StateMode { get; set; }
    public ReactiveCommand<Unit, Task> CultureMode { get; set; }
    public ReactiveCommand<Unit, Task> IdeoformMode { get; set; }
    public ReactiveCommand<Unit, Task> InstitutionMode { get; set; }
    public ReactiveCommand<Unit, Task> MachineMode { get; set; }
    public ReactiveCommand<Unit, Task> EcologyMode { get; set; }
    public ReactiveCommand<Unit, Task> HivemindMode { get; set; }
    public ReactiveCommand<Unit, Task> UndeadMode { get; set; }

    private async Task _observerMode()
    {
        var win = new ObserverPosition();
        win.Show();
        _window.Close();
    }
    
    private async Task _entrepreneurMode(){}
    private async Task _stateMode(){}
    private async Task _cultureMode(){}
    private async Task _ideoformMode(){}
    private async Task _instituteMode(){}
    private async Task _machineMode(){}
    private async Task _ecologyMode(){}
    private async Task _hivemindMode(){}
    private async Task _undeadMode()
    {
        
    }
}