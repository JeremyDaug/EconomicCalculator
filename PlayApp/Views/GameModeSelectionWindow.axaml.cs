using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PlayApp.Views;

public partial class GameModeSelectionWindow : Window
{
    public GameModeSelectionWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}