using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicSim.Helpers
{
    internal class TagData<T> : ITagData<T> where T : Enum
    {


        public TagData()
        {
            data = new List<object>();
        }

        public TagData(T tag,  params object[] data )
        {
            if (data == null)
                this.data = new List<object>();
            else
                this.data = new List<object>(data);
            Tag = tag;
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
