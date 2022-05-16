using System.Collections.ObjectModel;
using EconomicSim.Objects;
using EconomicSim.Objects.Wants;

namespace AvaEditorUI.ViewModels;

public class WantListViewModel : ViewModelBase
{
    private IDataContext dataContext;

    public WantListViewModel()
    {
        this.dataContext = DataContextFactory.GetDataContext;

        WantList = new ObservableCollection<IWant>(dataContext.Wants.Values);
    }
    
    public ObservableCollection<IWant> WantList { get; set; }
}