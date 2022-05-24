using AvaEditorUI.Models;
using AvaEditorUI.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvaEditorUI.Views;

public partial class SkillGroupEditorWindow : Window
{
    private SkillGroupEditorViewModel vm;
    public SkillGroupEditorWindow()
    {
        InitializeComponent();

        vm = new SkillGroupEditorViewModel(this);

        DataContext = vm;
#if DEBUG
        this.AttachDevTools();
#endif
    }
    
    public SkillGroupEditorWindow(SkillGroupEditorModel group)
    {
        InitializeComponent();

        vm = new SkillGroupEditorViewModel(this, group);

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