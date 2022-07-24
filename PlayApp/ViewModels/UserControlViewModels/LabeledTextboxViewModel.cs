using Avalonia.Layout;
using ReactiveUI;

namespace PlayApp.ViewModels.UserControlViewModels;

public class LabeledTextboxViewModel : ViewModelBase
{
    private string _label = "";
    private string _content = "";

    public LabeledTextboxViewModel()
    {
        
    }
    
    public Orientation Orientation { get; set; }

    public string Label
    {
        get => _label;
        set => this.RaiseAndSetIfChanged(ref _label, value);
    }

    public string Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }
}