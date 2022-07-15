using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlayApp.ViewModels;

namespace PlayApp.Views;

public partial class ObserverPosition : Window
{
    public ObserverPosition()
    {
        InitializeComponent();

        DataContext = new ObserverModeEntryViewModel(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}