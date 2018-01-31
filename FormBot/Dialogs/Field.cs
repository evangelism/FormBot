using FormBot.Evangelism;
using FormBot.Evangelism.Geo;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace FormBot.Dialogs
{
    [Serializable]
    public class Field
    {
        public Field(XElement X)
        {
            Name = X.Attribute("Name")?.Value;
            Type = X.Attribute("Type")?.Value;
            Text = X.Attribute("Text")?.Value;
            Condition = X.Attribute("Condition")?.Value;
            if (Type == null) Type = "string";
            if (X.Descendants("Choice") != null)
            {
                Options = (from z in X.Descendants("Choice")
                           select new Tuple<string, string>(z.Attribute("Value").Value, z.Value)).ToArray();
            }
            if (Type == "bool")
            {
                Options = new Tuple<string, string>[]
                {
                    new Tuple<string, string>("Y","Да"),
                    new Tuple<string, string>("N","Нет")
                };
            }
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public Tuple<string, string>[] Options { get; set; }

        public string Condition { get; set; }

        public bool Set(Indexed X, object Value)
        {
            // First we need to figure out the key value in case of Option field type
            if (Options != null && Options.Length > 0)
            {
                bool done = false;
                foreach (var x in Options)
                    if (x.Item2 == (string)Value)
                    {
                        Value = x.Item1;
                        done = true;
                        break;
                    }
                if (!done) return false;
            }
            if (Type == "int")
            {
                int t = 0;
                if (!int.TryParse((string)Value, out t)) return false;
                // else Value = t; -- uncomment this to convert value to int type
            }
            X[Name] = Value;
            return true;
        }

        public object Get(Indexed X)
        {
            return X[Name];
        }

        public async Task Render(IDialogContext context)
        {
            var msg = context.MakeMessage();
            msg.Text = Text;
            if (Options != null && Options.Length > 0) msg.Attachments = Options.Select(x => x.Item2).GenOptions();
            await context.PostAsync(msg);
        }

        public bool IsApplicable(Indexed X)
        {
            if (Condition == null || Condition.Length < 1) return true;
            var t = Condition.Split('=');
            var v = X[t[0]];
            if (v == null) return true;
            return v.ToString() == t[1];
        }
    }
}