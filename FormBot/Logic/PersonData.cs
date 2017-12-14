using FormBot.Dialogs;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.Logic 
{
    [Serializable]
    public class PersonData : TableEntity, IEntityRetriever<PersonData>
    {
        public static Dictionary<string, PersonData> Store = new Dictionary<string, PersonData>();

        public PersonData() { }
        public PersonData(string xid)
        {
            PartitionKey = "PD";
            RowKey = xid;
            Store.Add(xid, this);
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public string Student { get; set; }
        public string YearGraduated { get; set; }

        public PersonData GetEntity(string id)
        {
            if (Store.ContainsKey(id)) return Store[id];
            else return new PersonData(id);
        }
    }
}