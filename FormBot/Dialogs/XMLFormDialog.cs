using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Xml.Linq;
using System.Linq;
using FormBot.Evangelism;
using System.Collections.Generic;

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
            if (Type == null) Type = "string";
            if (X.Descendants("Choice")!=null)
            {
                Options = (from z in X.Descendants("Choice")
                           select new Tuple<string, string>(z.Attribute("Value").Value, z.Value)).ToArray();
            }
            if (Type=="bool")
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

        public bool Set(object X, object Value)
        {
            // First we need to figure out the key value in case of Option field type
            if (Options!=null && Options.Length>0)
            {
                bool done = false;
                foreach(var x in Options)
                    if (x.Item2==(string)Value)
                    {
                        Value = x.Item1;
                        done = true;
                        break;
                    }
                if (!done) return false;
            }
            int t = 0;
            if (Type == "int" && !int.TryParse((string)Value, out t)) return false;
            else Value = t;
            X.GetType().GetProperty(Name).SetValue(X, Value);
            return true;
        }

        public object Get(object X)
        {
            return X.GetType().GetProperty(Name).GetValue(X);
        }

        public async Task Render(IDialogContext context)
        {
            var msg = context.MakeMessage();
            msg.Text = Text;
            if (Options != null && Options.Length>0) msg.Attachments = Options.Select(x=>x.Item2).GenOptions();
            await context.PostAsync(msg);
        }
    }

    public interface IEntityRetriever<T>
    {
        T GetEntity(string id);
    }

    [Serializable]
    public class XMLFormDialog<T> : IDialog<T> where T: IEntityRetriever<T>, new()
    {
        Field[] Fields { get; }
        Field CurrentField { get; set; }
        T Object { get; set; }
        public XMLFormDialog()
        {
            var n = typeof(T).Name;
            var xdoc = XDocument.Load(System.Web.HttpContext.Current.Request.MapPath($"~/XML/{n}.xml"));
            Fields = (from z in xdoc.Descendants("Field") select new Field(z)).ToArray();
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (Object==null)
            {
                var tmp = new T();
                Object = tmp.GetEntity(activity.From.Id);
            }

            if (CurrentField!=null)
            {
                if (CurrentField.Set(Object, activity.Text)) CurrentField = null;
                else await context.PostAsync("Ошибочное значение");
            }

            if (CurrentField == null)
            {
                foreach (var x in Fields)
                {
                    if (x.Get(Object) == null)
                    {
                        CurrentField = x;
                        break;
                    }
                }
            }
            if (CurrentField == null) context.Done(Object);
            else
            {
                await CurrentField.Render(context);
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}