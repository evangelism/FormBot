using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace FormBot.Evangelism
{
    public class DObject : DynamicObject, IObjectIndexer
    {
        public Dictionary<string,object> Properties { get; private set; }

        public DObject()
        {
            Properties = new Dictionary<string, object>();
        }

        public object this[string name]
        {
            get
            {
                if (!Properties.ContainsKey(name)) Properties.Add(name, null);
                return Properties[name];
            }
            set
            {
                if (!Properties.ContainsKey(name)) Properties.Add(name, value);
                else Properties[name] = value;
            }
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


    }
}