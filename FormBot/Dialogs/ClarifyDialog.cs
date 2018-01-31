using FormBot.Evangelism;
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
    public class ClarifyDialog : IDialog<string>
    {
        public string Message;
        public string[] Options;
        public string Back;

        public ClarifyDialog(string msg, IEnumerable<string> Opts, string back = "Назад")
        {
            Message = msg; Back = back;
            Options = (new string[] { back }.Union(Opts)).ToArray();
        }
        public async Task StartAsync(IDialogContext context)
        {
            var msg = context.MakeMenu(Message, Options);
            await context.PostAsync(msg);
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (activity.Text == Back) context.Done(string.Empty);
            else
            {
                foreach (var x in Options)
                {
                    if (activity.Text == x)
                    {
                        context.Done(x);
                        return;
                    }
                }
                await context.PostAsync("Ошибочное значение");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}