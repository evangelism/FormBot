using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FormBot.Evangelism.AzureStorage
{
    [Serializable]
    public class AzureTable
    {

        protected string conn;
        protected string tabname;

        [NonSerialized]
        private CloudTable _table;

        public CloudTable Table
        {
            get
            {
                if (_table == null)
                {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conn);
                    var client = storageAccount.CreateCloudTableClient();
                    _table = client.GetTableReference(tabname);
                    _table.CreateIfNotExists();
                }
                return _table;
            }
        }

        public AzureTable(string conn, string tabname)
        {
            this.conn = conn; this.tabname = tabname;
        }

        public void Insert(ElasticTableEntity E)
        {
            Table.Execute(TableOperation.Insert(E));
        }

        public void Update(ElasticTableEntity E)
        {
            Table.Execute(TableOperation.Replace(E));
        }

        public void Delete(string PartitionKey, string RowKey)
        {
            var E = new ElasticTableEntity();
            E.PartitionKey = PartitionKey;
            E.RowKey = RowKey;
            Table.Execute(TableOperation.Delete(E));
        }

        public ElasticTableEntity Get(string PartitionKey, string RowKey)
        {
            var query = new TableQuery<ElasticTableEntity>()
            .Where(TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, PartitionKey),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, RowKey)));
            return Table.ExecuteQuery(query).SingleOrDefault();
        }

    }
}