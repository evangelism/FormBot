// The code here has been mostly taken from http://pascallaurin42.blogspot.ru/2013/03/using-azure-table-storage-with-dynamic.html

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

namespace FormBot.Evangelism.AzureStorage
{
    [Serializable]
    public class ElasticTableEntity : DynamicObject, ITableEntity, Indexed
    {
        public ElasticTableEntity()
        {
            this.Properties = new Dictionary<string, object>();
        }

        public bool Exists(string name) => Properties.ContainsKey(name);

        public IDictionary<string, object> Properties { get; private set; }

        public object this[string key]
        {
            get
            {
                if (!this.Properties.ContainsKey(key))
                    this.Properties.Add(key, null);
                return this.Properties[key];
            }
            set
            {
                if (this.Properties.ContainsKey(key))
                    this.Properties[key] = value;
                else
                    this.Properties.Add(key, value);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var p in Properties.Keys)
            {
                sb.AppendLine($" {p}: {this[p]}");
            }
            return sb.ToString();
        }

        #region DynamicObject overrides

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }

        #endregion

        #region ITableEntity implementation

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string ETag { get; set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Properties = new Dictionary<string, object>();
            foreach(var p in properties)
            {
                Properties.Add(p.Key, GetValue(p.Value));
            }
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var props = new Dictionary<string, EntityProperty>();
            foreach(var p in Properties)
            {
                props.Add(p.Key, GetEntityProperty(p.Key, p.Value));
            }
            return props;
        }

        #endregion


        private EntityProperty GetEntityProperty(string key, object value)
        {
            if (value == null) return new EntityProperty((string)null);
            if (value.GetType() == typeof(byte[])) return new EntityProperty((byte[])value);
            if (value.GetType() == typeof(bool)) return new EntityProperty((bool)value);
            if (value.GetType() == typeof(DateTimeOffset)) return new EntityProperty((DateTimeOffset)value);
            if (value.GetType() == typeof(DateTime)) return new EntityProperty((DateTime)value);
            if (value.GetType() == typeof(double)) return new EntityProperty((double)value);
            if (value.GetType() == typeof(Guid)) return new EntityProperty((Guid)value);
            if (value.GetType() == typeof(int)) return new EntityProperty((int)value);
            if (value.GetType() == typeof(long)) return new EntityProperty((long)value);
            if (value.GetType() == typeof(string)) return new EntityProperty((string)value);
            throw new Exception("not supported " + value.GetType() + " for " + key);
        }

        private Type GetType(EdmType edmType)
        {
            switch (edmType)
            {
                case EdmType.Binary: return typeof(byte[]);
                case EdmType.Boolean: return typeof(bool);
                case EdmType.DateTime: return typeof(DateTime);
                case EdmType.Double: return typeof(double);
                case EdmType.Guid: return typeof(Guid);
                case EdmType.Int32: return typeof(int);
                case EdmType.Int64: return typeof(long);
                case EdmType.String: return typeof(string);
                default: throw new Exception("not supported " + edmType);
            }
        }

        private object GetValue(EntityProperty property)
        {
            switch (property.PropertyType)
            {
                case EdmType.Binary: return property.BinaryValue;
                case EdmType.Boolean: return property.BooleanValue;
                case EdmType.DateTime: return property.DateTimeOffsetValue;
                case EdmType.Double: return property.DoubleValue;
                case EdmType.Guid: return property.GuidValue;
                case EdmType.Int32: return property.Int32Value;
                case EdmType.Int64: return property.Int64Value;
                case EdmType.String: return property.StringValue;
                default: throw new Exception("not supported " + property.PropertyType);
            }
        }
    }
}