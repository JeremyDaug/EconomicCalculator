using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class JobEditorWindow : Window
{
    public JobEditorWindow()
    {
        DataContext = new JobEditorViewModel(this);
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public JobEditorWindow(JobModel original)
    {
        DataContext = new JobEditorViewModel(original, this);
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