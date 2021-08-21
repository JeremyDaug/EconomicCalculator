using System;
using System.ComponentModel;

namespace EditorInterface.Products
{
    public class WantWeight : INotifyPropertyChanged
    {
        private string _name;
        private decimal _satisfaction;

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

        public decimal Satisfaction
        {
            get => _satisfaction;
            set
            {
                if (_satisfaction != value)
                {
                    _satisfaction = value;
                    RaisePropertyChanged(nameof(Satisfaction));
                }
            }
        }

        private void RaisePropertyChanged(string caller)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
