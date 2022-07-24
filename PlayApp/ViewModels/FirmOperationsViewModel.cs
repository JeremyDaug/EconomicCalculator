using Avalonia.Controls;
using EconomicSim.Objects;
using EconomicSim.Objects.Firms;

namespace PlayApp.ViewModels;

public class FirmOperationsViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window _window;
    private Firm parent;
    
    public FirmOperationsViewModel()
    {
        
    }

    public FirmOperationsViewModel(Firm parent, Window window)
    {
        _window = window;
        this.parent = parent;
    }
}