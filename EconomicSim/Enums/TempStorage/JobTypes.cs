namespace EconomicSim.Enums
{
    public enum JobTypes
    {
        Crop, // Works on time tables and yearly cycles.
        Mine, // Works on raw labor input, arbitrary values of labor produce goods linearly scaled.
        Craft, // Relies on Resource Inputs and Labor only makes in fixed increments.
        Processing, // Works raw resources and labor into something else in arbitrary increments.
        Shipping, // Relies on capiatal to start, buys goods and moves them elsewhere in days.
        Service // Primarily Labor Driven, with some inputs possibly needed. 
    }
}
