using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Randomizer
{
    public interface IRandomizer
    {
        int Next();

        int Next(int maxValue);

        int Next(int minValue, int MaxValue);

        double NextDouble();

        void NextByte(byte[] buffer);

        double DoubleBetweenValues(double min, double max);

        double Normal(double mean, double stdDev);

        double Normal();

        double Exponential(double rate);
    }
}
