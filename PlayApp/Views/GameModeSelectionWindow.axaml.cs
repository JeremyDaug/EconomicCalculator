using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PlayApp.ViewModels;

namespace PlayApp.Views;

public partial class GameModeSelectionWindow : Window
{
    public GameModeSelectionWindow()
    {
        InitializeComponent();

        DataContext = new GameModeSelectionViewModel(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}