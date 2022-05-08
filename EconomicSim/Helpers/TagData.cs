namespace EconomicSim.Helpers
{
    internal class TagData<T> : ITagData<T> where T : Enum
    {
        public TagData()
        {
            Parameters = new Dictionary<string, object>();
        }

        public TagData(T tag,  Dictionary<string, object> data = null)
        {
            if (data == null)
                this.Parameters = new Dictionary<string, object>();
            else
                this.Parameters = new Dictionary<string, object>(data);
            Tag = tag;
        }

        public Dictionary<string, object> Parameters;

        public object this[string i] 
        {
            get
            {
                return Parameters[i];
            }
            set
            {
                Parameters[i] = value;
            } 
        }

        public T Tag { get; set; }

        public int Count()
        {
            return Parameters.Count;
        }

        public void Add(string key, object value) => Parameters.Add(key, value);
    }
}
