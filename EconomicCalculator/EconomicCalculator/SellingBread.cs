using EconomicCalculator.Common;
using EconomicCalculator.Common.Processes;
using EconomicCalculator.Common.Resource;
using EconomicCalculator.Common.Sources;
using System;
using System.Collections.Generic;

namespace EconomicCalculator
{
    public class SellingBread
    {
        // from chapter 1 - bread

        public void ToMakeBread()
        {
            var manager = new Manager();
            var WheatField = new Crop("WheatField", "Basic", CropType.Grain, "Wheat", 120,
                new List<string> { "Wheat" }, new List<double> { 480 }, new List<double> { 0 }, "Spring", 0, "Autumn");

            var Wheat = WheatField.GetProduct("Wheat");
        }
    }
}