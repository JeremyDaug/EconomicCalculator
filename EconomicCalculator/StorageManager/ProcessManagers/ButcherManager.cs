using EconomicCalculator.Common.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace StorageManager.ProcessManagers
{
    public class ButcherManager
    {
        public List<Butcher> Butchers { get; set; } = new List<Butcher>();

        public IList<Butcher> LoadButchers(string filename)
        {

        }

        public void SaveButchers(string fileLocation, IList<Butcher> butchers)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
                throw new ArgumentException("Is Null or Whitespace", nameof(fileLocation));
            if (fileLocation.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                throw new ArgumentException("Invalid Filename", nameof(fileLocation));
            if (butchers == null) throw new ArgumentNullException(nameof(butchers));

            if (!File.Exists(fileLocation)) File.Create(fileLocation);

            XmlSerializer serializer = new XmlSerializer(typeof(ButcherManager));

            using (StreamWriter writer = new StreamWriter(filename))
                serializer.Serialize(writer, Butchers);

            return Butchers;
        }
    }
}
