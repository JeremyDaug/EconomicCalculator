using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class ProcessEditorWindow : Window
{
    public ProcessEditorWindow()
    {
        DataContext = new ProcessEditorViewModel(this);
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    public ProcessEditorWindow(ProcessModel model)
    {
        DataContext = new ProcessEditorViewModel(model, this);
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