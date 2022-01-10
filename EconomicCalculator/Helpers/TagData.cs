using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Helpers
{
    internal class TagData<T> : ITagData<T> where T : Enum
    {
        public TagData()
        {
            data = new List<object>();
        }

        private List<object> data;

        public object this[int i] 
        {
            get
            {
                return data[i];
            }
            set
            {
                data[i] = value;
            } 
        }

        public T Tag { get; set; }

        public int Count()
        {
            return data.Count;
        }

        public void Add(object value) => data.Add(value);
    }
}
