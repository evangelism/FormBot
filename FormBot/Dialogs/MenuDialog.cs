using FormBot.Evangelism;
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
        public MenuDialog(IStore<T> Store)
        {
            this.Store = Store;
        }
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var obj = Store.Get(activity.From.Id);
            await context.PostAsync("Ваши данные:" + obj.ToString());
            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }

    }
}