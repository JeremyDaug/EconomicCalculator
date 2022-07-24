using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlayApp.ViewModels.UserControlViewModels;

namespace PlayApp.UserControls;

public partial class LabeledTextbox : UserControl
{
    private LabeledTextboxViewModel vm; 
    public LabeledTextbox()
    {
        InitializeComponent();
        vm = new LabeledTextboxViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public string Orientation => vm.Label;
}