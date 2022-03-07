namespace EconomicCalculator.DTOs.Products
{
    public enum BuildingType
    {
        Housing, // A place where populations live. Low maintenance to 0 maintenance cost. Storage for pop needs.
        Warehouse, // A Place where Products can be stored. Moderate Maintenance cost. Large place for goods to be stored.
        Factory // A Place where people work. High Maintenance cost usually. Small storage for outputs.
    }
}