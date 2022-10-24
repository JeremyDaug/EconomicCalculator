namespace EconomicSim.Helpers;

public static class HelperExtensions
{
    /// <summary>
    /// Either adds a key and it's value to a dictionary, or if the key already exists
    /// it adds the value to the existing value.
    /// </summary>
    /// <param name="dict">The Dict to add or include it to.</param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TKey"></typeparam>
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