using FormBot.Evangelism.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.Evangelism.AzureStorage
{
    [Serializable]
    public class AzureStore<T> : IStore<T> where T: ElasticTableEntity, new()
    {
        public AzureTable Table { get; set; }
        public string PartitionKey { get; set; }
        public AzureStore(AzureTable Table, string PartitionKey)
        {
            this.Table = Table;
            this.PartitionKey = PartitionKey;
        }

        public bool Exists(string id)
        {
            var res = Table.Get(PartitionKey, id);
            return (res != null);
        }

        public T Get(string id)
        {
            var res = Table.Get(PartitionKey, id) as T;
            if (res==null)
            {
                res = new T();
                res.PartitionKey = PartitionKey;
                res.RowKey = id;
                Table.Insert(res);
            }
            return res;
        }

        public void Remove(string id)
        {
            Table.Delete(PartitionKey, id);
        }

        public void Update(string id, T obj)
        {
            // id is ignored here, we assume it exists inside obj
            Table.Update(obj);
        }

    }
}