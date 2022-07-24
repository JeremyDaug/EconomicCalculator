using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class SkillEditorWindow : Window
{
    public SkillEditorWindow()
    {
        InitializeComponent();

        var vm = new SkillEditorViewModel(this);

        DataContext = vm;
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public SkillEditorWindow(SkillEditorModel skill)
    {
        InitializeComponent();

        var vm = new SkillEditorViewModel(this, skill);

        DataContext = vm;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}