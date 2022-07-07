using System.Threading.Tasks;
using System.Timers;
using EconomicSim.Objects;
using ReactiveUI;

namespace PlayApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private decimal _progressValue;
        private string _text = "Loading Common Set.";
        private IDataContext dc = DataContextFactory.GetDataContext;

        public MainWindowViewModel()
        {
            // begin loading
            Task.Factory.StartNew(LoadDataCommon);
        }

        public async Task LoadDataCommon()
        {
            await Task.Delay(1000);
            ProgressValue = 50;
            await Task.Delay(1000);
            ProgressValue = 100;
            Text = "Loaded! Select Save.";
            
            
        }
        
        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }

        public decimal ProgressValue
        {
            get => _progressValue;
            set => this.RaiseAndSetIfChanged(ref _progressValue, value);
        }
    }
}