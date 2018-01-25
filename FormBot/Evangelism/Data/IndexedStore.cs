using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.Evangelism.Data
{
    public interface IStore<T> where T: new()
    {
        T Get(string id);
    }

    [Serializable]
    public class MemoryStore<T> : IStore<T> where T: new()
    {
        public static Dictionary<string, T> Objects = new Dictionary<string, T>();
        public T Get(string id)
        {
            if (!Objects.ContainsKey(id)) Objects.Add(id, new T());
            return Objects[id];
        }

    }

}