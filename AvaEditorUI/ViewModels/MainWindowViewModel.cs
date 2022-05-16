using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using AvaEditorUI.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using EconomicSim.Objects;
using ReactiveUI;

namespace AvaEditorUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public IDataContext Context = DataContextFactory.GetDataContext;

        public string Greeting => "Economic Simulator: Data and Save Editor";
    }
}