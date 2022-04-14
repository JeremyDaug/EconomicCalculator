using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EditorInterface.Firms
{
    public class ProductDecimalPair : INotifyPropertyChanged
    {
        private string product;
        private decimal price;

        public string Product
        {
            get { return product; }
            set
            {
                if (product != value)
                {
                    product = value;
                    RaisePropertyChanged();
                }
            }
        }

        public decimal Price
        {
            get { return price; }
            set
            {
                if (price != value)
                {
                    price = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}