namespace EconomicSim.Helpers;

public class PropertyTriple
{
    private decimal _reserved;
    private decimal _available;
    private decimal _total;

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
            _available = _reserved - _total;
        }
    }

    public decimal Reserved
    {
        get => _reserved;
        set
        {
            _reserved = value;
            _available = _total - _reserved;
        }
    }

    public decimal Available
    {
        get => _available;
        set
        {
            _available = value;
            _reserved = _total - _available;
        }
    }
}