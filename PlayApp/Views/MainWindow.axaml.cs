using System.Collections.Generic;
using Avalonia.Controls;
using EconomicSim.Objects;

namespace PlayApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            IDataContext? dataContext = DataContextFactory.GetDataContext;
            dataContext.LoadData(new List<string>{"Common"});
        }
    }
}