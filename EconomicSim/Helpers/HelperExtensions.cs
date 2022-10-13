namespace EconomicSim.Helpers;

public static class HelperExtensions
{
    public static void AddOrInclude<TKey>(this IDictionary<TKey, decimal> dict,
        TKey key, decimal value)
    {
        if (dict.ContainsKey(key))
            dict[key] += value;
        else
            dict[key] = value;
    }

    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> enumerable) where T : class
    {
        return enumerable.Where(x => x != null).Select(x => x!);
    }
}