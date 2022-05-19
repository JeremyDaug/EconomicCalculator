using EconomicSim.Objects;

namespace AvaEditorUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public IDataContext Context = DataContextFactory.GetDataContext;

        public string Greeting => "Economic Simulator: Data and Save Editor";
    }
}