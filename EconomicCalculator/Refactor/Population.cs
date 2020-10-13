using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;
using EconomicCalculator.Intermediaries;
using EconomicCalculator.Storage;
using EconomicCalculator.Storage.Jobs;

namespace EconomicCalculator.Generators
{
    internal class Population : IPopulation
    {
        public string Name { get; set; }

        public string VariantName { get; set; }

        public IMarket Market { get; set; }

        public JobCategory JobCategory { get; set; }

        public string JobName { get; set; }

        public IJob Job { get; set; }

        public int Count { get; set; }

        public IList<ICurrency> Currencies { get; set; }

        public IDictionary<string, double> CurrencyAmounts { get; set; }

        public IList<IProduct> GoodsForSale { get; set; }

        public IDictionary<string, double> GoodAmounts { get; set; }

        public IList<IProduct> LifeNeeds { get; set; }

        public IDictionary<string, double> LifeNeedAmounts { get; set; }

        /// <summary>
        /// The labor that remains from the previous day, allows for small pops to work over multiple days.
        /// </summary>
        private double RemainingLabor;

        // Job Timetable section reservation.

        public Population()
        {
            Currencies = new List<ICurrency>();
            CurrencyAmounts = new Dictionary<string, double>();
            GoodsForSale = new List<IProduct>();
            GoodAmounts = new Dictionary<string, double>();
            LifeNeeds = new List<IProduct>();
            LifeNeedAmounts = new Dictionary<string, double>();
        }

        public Task DoWork()
        {
            // Get a rounded labor value for jobs that require more than one day.
            // TODO

            // Send Information to Job and let it handle it.
            //var results = Job.Work(GoodAmounts, Count);

            return Task.CompletedTask;
        }

        public override string ToString()
        {
            var result = string.Format("Name: {0}\n" +
                "Variant Name: {1}\n" +
                "Market: {2}\n" +
                "Job Category: {3}\n" +
                "Job Name: {4}\n" +
                "Job: {5}\n" +
                "Population: {6}\n",
                Name, VariantName, Market.Name, JobCategory, JobName, Job.Name, Count);

            // Currencies
            result += "Currencies:\n";
            foreach (var currency in Currencies)
            {
                result += string.Format("\t{0}: {1} {2}\n",
                    currency.Name, CurrencyAmounts[currency.Name], currency.Symbol);
            }

            // Goods for sale
            result += "Available Goods:\n";
            foreach (var goods in GoodsForSale)
            {
                result += string.Format("\t{0}: {1} {2}\n",
                    goods.Name, GoodAmounts[goods.Name], goods.UnitName);
            }

            // Needs
            result += "Life Needs:\n";
            foreach (var need in LifeNeeds)
            {
                result += string.Format("\t{0}: {1} {2}\n",
                    need.Name, LifeNeedAmounts[need.Name], need.UnitName);
            }

            result += "--------------------\n";

            return result;
        }
    }
}
