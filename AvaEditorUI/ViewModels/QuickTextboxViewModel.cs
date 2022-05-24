using ReactiveUI;

namespace AvaEditorUI.ViewModels;

public class QuickTextboxViewModel : ViewModelBase
{
    private string _value;

    public string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
}