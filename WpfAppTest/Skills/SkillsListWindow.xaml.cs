using EconDTOs;
using EconDTOs.DTOs.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EditorInterface.Skills
{
    /// <summary>
    /// Interaction logic for SkillsListWindow.xaml
    /// </summary>
    public partial class SkillsListWindow : Window
    {
        public IList<ISkill> skills;
        public IList<ISkillGroup> skillGroups;

        private Manager manager;

        public SkillsListWindow()
        {
            InitializeComponent();

            manager = Manager.Instance;

            skills = manager.Skills.Values.ToList();
            skillGroups = manager.SkillGroups.Values.ToList();

            SkillList.ItemsSource = skills;
            GroupList.ItemsSource = skillGroups;
        }

        private void NewSkill(object sender, RoutedEventArgs e)
        {
            var newSkill = new Skill
            {
                Id = manager.NewSKillId,
                Name = "",
                Description = "Info"
            };

            Window win = new SkillEditorWindow(newSkill);

            win.ShowDialog();
            SkillList.ItemsSource = manager.Skills.Values.ToList();
            SkillList.Items.Refresh();
        }

        private void EditSkill(object sender, RoutedEventArgs e)
        {
            var selected = ((Skill)SkillList.SelectedItem);
            if (selected == null)
                return;

            Window win = new SkillEditorWindow(selected);

            win.ShowDialog();
            SkillList.ItemsSource = manager.Skills.Values.ToList();
            SkillList.Items.Refresh();
        }

        private void CopySkill(object sender, RoutedEventArgs e)
        {
            var selected = ((Skill)SkillList.SelectedItem);
            if (selected == null)
                return;

            var copy = new Skill
            {
                Id = manager.NewSKillId,
                Name = selected.Name,
                Groups = selected.Groups.ToList(),
                GroupStrings = selected.GroupStrings.ToList(),
                Related = selected.Related.ToDictionary(x => x.Key, y => y.Value),
                RelatedStrings = selected.RelatedStrings.ToDictionary(x => x.Key, x => x.Value),
                Description = selected.Description
            };

            Window win = new SkillEditorWindow(selected);

            win.ShowDialog();
            SkillList.ItemsSource = manager.Skills.Values.ToList();
            SkillList.Items.Refresh();
        }

        private void NewGroup(object sender, RoutedEventArgs e)
        {
            var newGroup = new SkillGroup
            {
                Id = manager.NewSkillGroupId,
                Name = "",
                Default = 0.5m,
                Description = "Info"
            };
            Window win = new SkillGroupEditor(newGroup);

            win.ShowDialog();
            GroupList.ItemsSource = manager.SkillGroups.Values;
            GroupList.Items.Refresh();
        }

        private void EditGroup(object sender, RoutedEventArgs e)
        {
            var selected = ((SkillGroup)GroupList.SelectedItem);
            if (selected == null)
                return;

            Window win = new SkillGroupEditor(selected);

            win.ShowDialog();
            GroupList.ItemsSource = manager.SkillGroups.Values;
            GroupList.Items.Refresh();
        }

        private void CopyGroup(object sender, RoutedEventArgs e)
        {
            var selected = ((SkillGroup)GroupList.SelectedItem);
            if (selected == null)
                return;

            var copy = new SkillGroup
            {
                Id = manager.NewSkillGroupId,
                Name = selected.Name,
                Default = selected.Default,
                Description = selected.Description
            };

            Window win = new SkillGroupEditor(copy);

            win.ShowDialog();
            GroupList.ItemsSource = manager.SkillGroups.Values;
            GroupList.Items.Refresh();
        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {
            // Get all skill groups
            manager.SaveSkillGroups(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonSkillGroups.json");

            // Get All Skills
            manager.SaveSkills(@"D:\Projects\EconomicCalculator\EconomicCalculator\Data\CommonSkills.json");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            manager.GenerateSkillLabors();

            MessageBox.Show("Generation Completd, be sure to go to Products and confirm them.");
        }
    }
}
