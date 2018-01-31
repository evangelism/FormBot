using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.Evangelism
{
    public interface Indexed
    {
        object this[string name] { get; set; }
        bool Exists(string name);
    }
}