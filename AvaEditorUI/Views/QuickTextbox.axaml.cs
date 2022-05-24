using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class QuickTextbox : Window
{
    public QuickTextboxViewModel vm;
    public QuickTextbox()
    {
        InitializeComponent();

        vm = new QuickTextboxViewModel();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}