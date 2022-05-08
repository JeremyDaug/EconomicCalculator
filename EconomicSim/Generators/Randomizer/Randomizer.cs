namespace EconomicSim.Randomizer
{
    /// <summary>
    /// Randomizer class, will not be tested as this class is just a wrapper for 
    /// Random to make testing easier and allow this to be passed around.
    /// </summary>
    internal class Randomizer : IRandomizer
    {
        private Random rand;

        public Randomizer()
        {
            rand = new Random();
        }

        public Randomizer(int seed)
        {
            rand = new Random(seed);
        }

        public int Next()
        {
            return rand.Next();
        }

        public int Next(int maxValue)
        {
            return rand.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return rand.Next(minValue, maxValue);
        }

        public double NextDouble()
        {
            return rand.NextDouble();
        }

        public double NextDouble(double max)
        {
            return NextDouble() * max;
        }

        public double NextDouble(double min, double max)
        {
            return NextDouble() * (max - min) + min;
        }

        public void NextByte(byte[] buffer)
        {
            rand.NextBytes(buffer);
        }

        public double Normal(double mean, double stdDev)
        {
            var u1 = 1.0 - NextDouble();
            var u2 = 1.0 - NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1))
                * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        public double Normal()
        {
            return Normal(0, 1);
        }

        public double Exponential(double rate)
        {
            return Math.Log(1 - NextDouble()) / (-rate);
        }
    }
}
