using EconomicCalculator;
using EconomicCalculator.Storage.Skills;
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
    /// Interaction logic for SkillGroupEditor.xaml
    /// </summary>
    public partial class SkillGroupEditor : Window
    {
        private SkillGroup group;


        public SkillGroupEditor()
        {
            InitializeComponent();
        }

        public SkillGroupEditor(SkillGroup g)
        {
            InitializeComponent();
            group = g;

            GroupId.Text = group.Id.ToString();
            GroupName.Text = group.Name;
            GroupDefault.Text = group.Default.ToString();
            GroupDesc.Text = group.Description;
        }

        private void CommitGroup(object sender, RoutedEventArgs e)
        {
            var newGroup = new SkillGroup
            {
                Id = group.Id,
                Name = GroupName.Text,
                Default = decimal.Parse(GroupDefault.Text),
                Description = GroupDesc.Text
            };
            Manager.Instance.SkillGroups[newGroup.Id] = newGroup;
        }
    }
}
