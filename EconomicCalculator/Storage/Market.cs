using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Intermediaries;

namespace EconomicCalculator.Storage
{
    internal class Market : IMarket
    {
        public Guid Id { get; }

        #region GeneralInfo

        public string Name { get; set; }

        public double TotalPopulation { get; set; }

        public int Territory { get; set; }

        #endregion GeneralInfo

        #region InfoDetails

        public IPopulations Populations { get; set; }

        // Territory Breakdowns

        #endregion InfoDetails

        #region TheMarket

        public IProductAmountCollection ProductSupply { get; set; }

        public IProductAmountCollection ProductDemand { get; set; }

        public IProductAmountCollection ProductPrices { get; set; }

        public IProductAmountCollection ProductionCapacity { get; set; }

        public IList<IProduct> AcceptedCurrencies { get; set; }

        public IPopulationGroup MoneyChangers => throw new NotImplementedException();

        public void BuyPhase()
        {
            throw new NotImplementedException();
        }

        public void ProductionPhase()
        {
            throw new NotImplementedException();
        }

        public void RecalculatePrices()
        {
            throw new NotImplementedException();
        }

        #endregion TheMarket

        public bool Equals(IMarket other)
        {
            throw new NotImplementedException();
        }

        public string PrintCrops()
        {
            throw new NotImplementedException();
        }

        public string PrintCurrencies()
        {
            throw new NotImplementedException();
        }

        public string PrintMines()
        {
            throw new NotImplementedException();
        }

        public string PrintPops()
        {
            throw new NotImplementedException();
        }

        public string PrintProcesses()
        {
            throw new NotImplementedException();
        }

        public string PrintProducts()
        {
            throw new NotImplementedException();
        }

        public double GetPrice(IProduct product, double amount)
        {
            throw new NotImplementedException();
        }

        public double ExchangeCash(IProduct source, double amount, IProduct target)
        {
            throw new NotImplementedException();
        }

        public IProductAmountCollection StartTransaction(IPopulationGroup populationGroup, IProduct product, double amount)
        {
            throw new NotImplementedException();
        }
    }
}
