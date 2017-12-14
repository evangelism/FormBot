using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Xml.Linq;
using System.Linq;

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
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public Tuple<string, string>[] Options { get; set; }

        public void Set(object X, object Value)
        {
            X.GetType().GetProperty(Name).SetValue(X, Value);
        }

        public object Get(object X)
        {
            return X.GetType().GetProperty(Name).GetValue(X);
        }

        public async Task Render(IDialogContext context)
        {
            await context.PostAsync(Text);
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
                CurrentField.Set(Object, activity.Text);
            }

            CurrentField = null;
            foreach(var x in Fields)
            {
                if (x.Get(Object)==null)
                {
                    CurrentField = x;
                    break;
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