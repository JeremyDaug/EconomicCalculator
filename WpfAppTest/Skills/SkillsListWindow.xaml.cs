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
    /// Interaction logic for SkillsListWindow.xaml
    /// </summary>
    public partial class SkillsListWindow : Window
    {
        public IList<ISkill> skills;
        public IList<ISkillGroup> skillGroups;

        public SkillsListWindow()
        {
            InitializeComponent();

            skills = Manager.Instance.Skills.Values.ToList();
            skillGroups = Manager.Instance.SkillGroups.Values.ToList();

            SkillList.ItemsSource = skills;
            GroupList.ItemsSource = skillGroups;
        }

        private void NewSkill(object sender, RoutedEventArgs e)
        {

        }

        private void EditSkill(object sender, RoutedEventArgs e)
        {

        }

        private void CopySkill(object sender, RoutedEventArgs e)
        {

        }

        private void NewGroup(object sender, RoutedEventArgs e)
        {

        }

        private void EditGroup(object sender, RoutedEventArgs e)
        {

        }

        private void CopyGroup(object sender, RoutedEventArgs e)
        {

        }

        private void SaveToFile(object sender, RoutedEventArgs e)
        {

        }
    }
}
