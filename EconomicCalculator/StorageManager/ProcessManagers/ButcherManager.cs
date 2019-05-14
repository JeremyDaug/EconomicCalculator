using EconomicCalculator.Common.Processes;
using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace StorageManager.ProcessManagers
{
    public class ButcherManager
    {
        public IList<Butcher> LoadButchers(string filename)
        {
            var result = new List<Butcher>();
            var doc = XDocument.Load(filename);

            foreach (XElement element in doc.Descendants()
                .Where(x => x.Name.LocalName == "butcher")
                )
            {
                var Name = element.Descendants()
                    .Single(x => x.Name.LocalName == "name").Value;
                var Variant = element.Descendants()
                    .SingleOrDefault(x => x.Name.LocalName == "variant").Value;
                var Animal = element.Descendants()
                    .Single(x => x.Name.LocalName == "animal").Value;
                var PriceMultiplier = Double
                    .Parse(element
                        .Descendants()
                        .Single(x => x.Name.LocalName == "priceMultiplier").Value);
                var temp = new Butcher
                {
                    Name = Name,
                    Variant = Variant,
                    Animal = Animal,
                    PriceMultiplier = PriceMultiplier
                };
            }

            return result;
        }
    }
}
