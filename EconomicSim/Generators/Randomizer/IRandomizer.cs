namespace EconomicSim.Randomizer
{
    public interface IRandomizer
    {
        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        int Next();

        /// <summary>
        /// Returns a non-Negative random integer less than <paramref name="maxValue"/>.
        /// </summary>
        /// <param name="maxValue"></param>
        int Next(int maxValue);

        /// <summary>
        /// Returns a random integer between <paramref name="minValue"/> and <paramref name="MaxValue"/>.
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="MaxValue"></param>
        int Next(int minValue, int MaxValue);

        /// <summary>
        /// Produces a double in the range of [0,1).
        /// </summary>
        double NextDouble();

        /// <summary>
        /// Produces a random double from [0, <paramref name="max"/>).
        /// </summary>
        /// <param name="max"></param>
        double NextDouble(double max);

        /// <summary>
        /// Produces a random double from [<paramref name="min"/>, <paramref name="max"/>).
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        double NextDouble(double min, double max);

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer"></param>
        void NextByte(byte[] buffer);

        /// <summary>
        /// Retrieves a number across a normal distribution.
        /// </summary>
        /// <param name="mean">The midpoint of the distribution.</param>
        /// <param name="stdDev">The spread.</param>
        /// <returns></returns>
        double Normal(double mean, double stdDev);

        /// <summary>
        /// Retrieves a number a cross a normal distribution with 
        /// mean of 0 and standard deviation of 1.
        /// </summary>
        double Normal();

        /// <summary>
        /// Retrieve a number across an exponential distribution.
        /// </summary>
        /// <param name="rate">The rate of the exponent.</param>
        double Exponential(double rate);
    }
}
