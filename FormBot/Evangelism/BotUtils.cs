using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.Evangelism
{
    public static class BotUtils
    {
        public static Attachment[] GenOptions(this IEnumerable<string> options)
        {
            var Card = new HeroCard()
            {
                Buttons = (from x in options
                           select new CardAction
                           {
                               Type = "imBack",
                               Value = x,
                               Title = x
                           }).ToArray()
            };
            return new Attachment[] { Card.ToAttachment() };
        }

        public static IMessageActivity MakeMenu(this IDialogContext ctx, string txt, string[] opts)
        {
            var msg = ctx.MakeMessage();
            msg.Text = txt;
            msg.Attachments = opts.GenOptions();
            return msg;
        }

    }
}