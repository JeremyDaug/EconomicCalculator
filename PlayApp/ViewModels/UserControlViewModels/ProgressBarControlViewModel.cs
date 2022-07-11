using ReactiveUI;

namespace PlayApp.ViewModels.UserControlViewModels;

public class ProgressBarControlViewModel : ViewModelBase
{
    private string _textLabel = "Progress Bar Label";
    private decimal _progressBarValue = 50;

    public ProgressBarControlViewModel()
    {
        
    }

    public string TextLabel
    {
        get => _textLabel;
        set => this.RaiseAndSetIfChanged(ref _textLabel, value);
    }

    public decimal ProgressBarValue
    {
        get => _progressBarValue;
        set => this.RaiseAndSetIfChanged(ref _progressBarValue, value);
    }
}