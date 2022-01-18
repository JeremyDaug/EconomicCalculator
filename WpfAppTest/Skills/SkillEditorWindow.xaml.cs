using EconomicCalculator;
using EconomicCalculator.DTOs.Skills;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Editor.Skills
{
    /// <summary>
    /// Interaction logic for SkillEditorWindow.xaml
    /// </summary>
    public partial class SkillEditorWindow : Window
    {
        private DTOManager manager;
        private SkillDTO skill;

        private List<string> availableGroups;
        private List<string> availableRelations;
        private List<SelecetGroups> Groups;

        private List<SkillRelation> relations;

        private bool Reciprocate;

        public SkillEditorWindow()
        {
            InitializeComponent();
        }

        public SkillEditorWindow(SkillDTO skill)
        {
            InitializeComponent();
            manager = DTOManager.Instance;
            this.skill = skill;

            // set easy data.
            SkillId.Text = skill.Id.ToString();
            SkillName.Text = skill.Name;
            SkillGroups.ItemsSource = skill.GroupStrings;
            SkillDesc.Text = skill.Description;

            // Set Relations
            relations = new List<SkillRelation>();
            foreach (var rel in skill.RelatedStrings)
            {
                relations.Add(new SkillRelation{
                    Name = rel.Key,
                    Strength = rel.Value
                });
            }
            SkillRelations.ItemsSource = relations;

            availableRelations = manager.Skills.Values
                .Where(x => x.Id != skill.Id)
                .Select(x => x.Name).ToList();
            RelatedName.ItemsSource = availableRelations;

            // Set Groups
            Groups = new List<SelecetGroups>();
            foreach (var group in skill.GroupStrings)
                Groups.Add(new SelecetGroups { Group = group });

            SkillGroups.ItemsSource = Groups;

            availableGroups = manager.SkillGroups.Values
                .Select(x => x.Name).ToList();
            GroupName.ItemsSource = availableGroups;
        }

        private void CommitSkill(object sender, RoutedEventArgs e)
        {
            // create new skill.
            var newSkill = new SkillDTO
            {
                Id = skill.Id,
                Name = SkillName.Text,
                Description = SkillDesc.Text
            };

            // Groups
            foreach (var group in Groups)
            {
                newSkill.GroupStrings.Add(group.Group);
                newSkill.Groups.Add(manager.GetSkillGroupByName(group.Group).Id);
            }

            // ensure no duplicates in groups
            if (newSkill.GroupStrings.GroupBy(x => x).Any(c => c.Count() > 1))
            {
                MessageBox.Show("Duplicate Group found. Please remove duplicate.", "Duplicate Group", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Related Skills
            // check for duplicates
            if (relations.Select(x => x.Name)
                .GroupBy(x => x)
                .Any(c => c.Count() > 1))
            {
                MessageBox.Show("Duplicate Skill Relation Found. Please remove the duplicate.", "Duplicate Skill", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (relations.Any())
            {
                newSkill.RelatedStrings = relations.ToDictionary(x => x.Name, x => x.Strength);
                newSkill.Related = relations.ToDictionary(x => manager.GetSkillByName(x.Name).Id, x => x.Strength);
            }

            manager.Skills[newSkill.Id] = newSkill;

            // if reciprocating connection, do it.
            if (Reciprocate)
            {
                foreach (var rel in newSkill.Related)
                {
                    manager.Skills[rel.Key].Related[newSkill.Id] = rel.Value;
                    manager.Skills[rel.Key].RelatedStrings[newSkill.Name] = rel.Value;
                }
            }
        }

        private void EqualSelected(object sender, RoutedEventArgs e)
        {
            Reciprocate = true;
        }

        private void OneWaySelected(object sender, RoutedEventArgs e)
        {
            Reciprocate = false;
        }
    }

    internal class SelecetGroups
    {
        public string Group { get; set; }
    }

    public class SkillRelation : INotifyPropertyChanged
    {
        private string _name;
        private decimal _strength;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (!string.Equals(value, _name))
                {
                    _name = value;
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        public decimal Strength
        {
            get
            {
                return _strength;
            }
            set
            {
                if (value != _strength)
                {
                    _strength = value;
                    RaisePropertyChanged(nameof(Strength));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}
