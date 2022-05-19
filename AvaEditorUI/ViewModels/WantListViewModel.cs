using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaEditorUI.Views;
using DynamicData;
using EconomicSim.Objects;
using EconomicSim.Objects.Wants;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class WantListViewModel : ViewModelBase
{
    private readonly IDataContext _dataContext;
    private bool _isButtonsEnable;

    public WantListViewModel()
    {
        this._dataContext = DataContextFactory.GetDataContext;

        WantList = new ObservableCollection<IWant>(_dataContext.Wants.Values);

        NewWant = ReactiveCommand.Create(CreateNewWant);
        EditWant = ReactiveCommand.Create(EditExistingWant);
        //CopyWant = ReactiveCommand.Create(CopyExistingWant);
        SaveWants = ReactiveCommand.Create(Save);
        IsButtonsEnable = true;
    }
    
    public IWant? Selection { get; set; }

    public bool IsButtonsEnable
    {
        get => _isButtonsEnable;
        set => this.RaiseAndSetIfChanged(ref _isButtonsEnable, value);
    }

    #region Commands

    public ReactiveCommand<Unit, Task> NewWant { get; }

    public ReactiveCommand<Unit, Task> EditWant { get; }

    //public ReactiveCommand<Unit, Task> CopyWant { get; }

    public ReactiveCommand<Unit, Unit> SaveWants { get; }

    private async Task CreateNewWant()
    {
        var win = new WantEditorWindow();
        await win.ShowDialog(Window);
        WantList.Clear();
        WantList.Add(_dataContext.Wants.Values);
    }

    private async Task  EditExistingWant()
    {
        if (Selection == null)
            return;

        var win = new WantEditorWindow(Selection);
        await win.ShowDialog(Window);
        WantList.Clear();
        WantList.Add(_dataContext.Wants.Values);
    }

    /*
    private async Task  CopyExistingWant()
    {
        throw new NotImplementedException();
    }*/

    void Save()
    {
        _dataContext.SaveWants();
        MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow("Saved.", "Wants Saved!",
                ButtonEnum.Ok, Icon.Info)
            .Show();
    }

    #endregion Commands
    
    public ObservableCollection<IWant> WantList { get; set; }
    public WantListWindow? Window { get; set; }
}