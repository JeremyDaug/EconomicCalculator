using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;
using EconomicCalculator.Randomizer;

namespace EconomicCalculator.Refactor.Storage.Products
{
    internal class Currency : Product, ICurrency
    {
        private readonly IRandomizer rand;

        public Currency(IRandomizer rand) : base(rand)
        {
            Trust = 0.5;
            Symbol = 'x';
            //Maintenance = new ProductAmountCollection();
        }

        public CashType CashType { get; set; }

        public double Trust { get; set; }

        public char Symbol { get; set; }

        public IProduct Backing { get; set; }

        public new bool Maintainable => false;

        //public new IReadOnlyProductAmountCollection Maintenance { get; }

        public double RawValue(IMarket market)
        {
            switch (CashType)
            {
                case CashType.Commodity:
                    return market.GetPrice(Backing);
                case CashType.Fiat:
                    return 0;
                case CashType.Minted:
                    return market.GetPrice(Backing);
                case CashType.Token:
                    return market.GetPrice(Backing);
                default:
                    throw new ArgumentOutOfRangeException("CashType not Valid");
            }
        }
    }
}
