using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Objects
{
    public class DataContextFactory
    {
        public static IDataContext GetDataContext => DataContext.Instance;
    }
}
