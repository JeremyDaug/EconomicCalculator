using System.ComponentModel;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class SkillListsWindow : Window
{
    public SkillListsWindow()
    {
        InitializeComponent();

        var vm = new SkillListsViewModel(this);

        DataContext = vm;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        var win = new MainWindow
        {
            DataContext = new MainWindowViewModel()
        };
        win.Show();
        base.OnClosing(e);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}