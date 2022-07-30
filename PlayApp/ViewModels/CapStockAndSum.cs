using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace PlayApp.ViewModels;

public class CapStockAndSum : INotifyPropertyChanged
{
    private string _item;
    private decimal _price;
    private decimal _estimatedResults;
    private decimal _stock;
    private decimal _expenditures;
    private decimal _gains;
    private decimal _used;

    public CapStockAndSum(string item, decimal stock = 0, decimal expenditures = 0, decimal used = 0, decimal gains = 0, decimal price = 0)
    {
        _item = item;
        Stock = stock;
        Expenditures = expenditures;
        Gains = gains;
        Price = price;
    }

    public string Item
    {
        get => _item;
        set
        {
            if (_item != value)
            {
                _item = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal Stock
    {
        get => _stock;
        set
        {
            if (_stock != value)
            {
                _stock = value;
                EstimatedResults = _stock - Expenditures + Gains;
                OnPropertyChanged();
            }
        }
    }

    public decimal Expenditures
    {
        get => _expenditures;
        set
        {
            if (_expenditures != value)
            {
                _expenditures = value;
                EstimatedResults = Stock - Expenditures + Gains;
                OnPropertyChanged();
            }
        }
    }

    public decimal Used
    {
        get => _used;
        set
        {
            if (_used != value)
            {
                _used = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal Gains
    {
        get => _gains;
        set
        {
            if (_gains != value)
            {
                _gains = value;
                EstimatedResults = Stock - Expenditures + Gains;
                OnPropertyChanged();
            }
        }
    }

    public decimal EstimatedResults
    {
        get => _estimatedResults;
        private set
        {
            if (_estimatedResults != value)
            {
                _estimatedResults = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}