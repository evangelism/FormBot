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

    }
}