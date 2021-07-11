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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppTest.Maps;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void OnCLick1(object sender, RoutedEventArgs e)
        {
            // confirm loading default data.
            var result = MessageBox.Show("Load Test Data Set?", "Yes or No", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                // if yes, then close opening window and open new window.
                // We start at planet window for now, this should start at
                // galaxy view for later testing.
                PlanetViewWindow EntryPoint = new PlanetViewWindow();

                EntryPoint.LoadData("TestData");

                // open the new window and close the next.
                EntryPoint.Show();
                this.Close();
            }
        }
    }
}
