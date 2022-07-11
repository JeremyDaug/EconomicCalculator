using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlayApp.ViewModels.UserControlViewModels;

namespace PlayApp.UserControls;

public partial class ProgressBarControl : UserControl
{
    private ProgressBarControlViewModel vm;
    public ProgressBarControl()
    {
        InitializeComponent();
        vm = new ProgressBarControlViewModel();
        DataContext = vm;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public string Label
    {
        get
        {
            return vm.TextLabel;
        }
        set
        {
            vm.TextLabel = value;
        }
    }

    public decimal Progress
    {
        get
        {
            return vm.ProgressBarValue;
        }
        set
        {
            vm.ProgressBarValue = value;
        }
    }
}