namespace EconomicSim.Objects.Government
{
    /// <summary>
    /// The possible locations for taxes and subsidies to be placed,
    /// Taxes on specific products are not included
    /// IE, consumption taxes or property taxes.
    /// but recorded elsewhere.
    /// </summary>
    public enum TaxTarget
    {
        /// <summary>
        /// Taxes come out as a percentage of wages.
        /// </summary>
        Income,
        /// <summary>
        /// Taxes are a percent of total owned wealth
        /// with some specific goods listed as 'wealth'.
        /// </summary>
        Wealth,
        /// <summary>
        /// A tax on the selling of goods.
        /// </summary>
        Sale,
        /// <summary>
        /// Taxes come out of firms profits
        /// </summary>
        Profit,
        /// <summary>
        /// Taxes are set on imports of goods.
        /// </summary>
        Tariff
    }
}
