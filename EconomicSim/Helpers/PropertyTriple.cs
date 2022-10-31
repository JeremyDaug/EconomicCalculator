namespace EconomicSim.Helpers;

public class PropertyTriple
{
    private decimal _reserved;
    private decimal _available;
    private decimal _total;
    private decimal _exhausted;

    public PropertyTriple(decimal total, decimal reserved = 0)
    {
        Total = total;
        Reserved = reserved;
    }
    
    public decimal Total
    {
        get => _total;
        set
        {
            _total = value;
            if (_reserved > _total)
                _reserved = _total; // if total has been reduced below the reserve, reduce reserve.
            _available = _total - _reserved - _exhausted;
        }
    }

    public decimal Reserved
    {
        get => _reserved;
        set
        {
            _reserved = value;
            _available = _total - _reserved - _exhausted;
        }
    }

    public decimal Exhausted
    {
        get => _exhausted;
        set
        {
            _exhausted = value;
            _available = _total - _reserved - _exhausted;
        }
    }

    public decimal Available
    {
        get => _available;
        set
        {
            _available = value;
            _reserved = _total - _available - _exhausted;
        }
    }
}