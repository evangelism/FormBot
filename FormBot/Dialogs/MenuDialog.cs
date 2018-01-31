using FormBot.Evangelism;
using FormBot.Evangelism.AzureStorage;
using FormBot.Evangelism.Data;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FormBot.Dialogs
{
    [Serializable]
    public class MenuDialog<T> : IDialog<T> where T : Indexed, new()
    {
        protected IStore<T> Store { get; set; }
        protected Indexed MyObject { get; set; }
        public MenuDialog(IStore<T> Store, Indexed MyObject)
        {
            this.Store = Store;
            this.MyObject = MyObject;
        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Ваши даные:" + MyObject.ToString());
            await context.PostAsync(context.MakeMenu("Что вы хотите сделать?", new string[] { "Очистить", "Редактировать" }));
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            switch(activity.Text)
            {
                case "Очистить":
                    Store.Remove(activity.From.Id);
                    await context.PostAsync("Данные удалены");
                    context.Call(
                        new XMLFormDialog<ElasticTableEntity>(
                          new AzureStore<ElasticTableEntity>(
                           new AzureTable(Config.ConnectionString, "PersonInfo"), "PersonInfo"), "PersonInfo"), MessageReceivedAsync);
                    break;
                default:
                    await context.PostAsync("Команда непонятна");
                    await context.PostAsync(context.MakeMenu("Что вы хотите сделать?", new string[] { "Очистить", "Редактировать" }));
                    context.Wait(MessageReceivedAsync);
                    break;
            }
        }

    }
}