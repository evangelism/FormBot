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
    public class GeoDialog : IDialog<string>
    {
        public GeoCity[] Cities;
        public GeoDialog(string fn)
        {
            var xdoc = XDocument.Load(System.Web.HttpContext.Current.Request.MapPath($"~/XML/{fn}.xml"));
            Cities = (from z in xdoc.Descendants("City")
                      select new GeoCity()
                      {
                          Name = z.Attribute("Name").Value,
                          Long = double.Parse(z.Attribute("Long").Value),
                          Lat = double.Parse(z.Attribute("Lat").Value),
                          Country = z.Attribute("Country").Value
                      }).ToArray();
        }
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var E = activity.Entities;
            if (E != null && E.Count > 0)
            {
                var lat = double.Parse(E[0].Properties["geo"]["latitude"].ToString());
                var lng = double.Parse(E[0].Properties["geo"]["longitude"].ToString());
                var res = GeoCity.GetClosestCities(Cities, lat, lng);

                if (res.Length <= 0)
                {
                    await context.PostAsync("Городов не найдено. Попробуйте ещё раз.");
                    context.Wait(MessageReceivedAsync);
                }
                else if (res.Length == 1)
                {
                    context.Done(res[0].Name);
                }
                else
                {
                    context.Call(new ClarifyDialog("Уточните город", res.Select(x => x.Name)), async (ctx, aw) =>
                           {
                               var r = await aw;
                               if (r == "") ctx.Wait(MessageReceivedAsync);
                               else ctx.Done(r);
                           });
                }
            } // end geoposition attached
            else
            {
                var res = GeoCity.GetSimilarCities(Cities, activity.Text);
                if (res == null || res.Length == 0)
                {
                    await context.PostAsync("Городов не найдено. Попробуйте ещё раз.");
                    context.Wait(MessageReceivedAsync);
                }
                else if (res.Length == 1)
                {
                    context.Done(res[0].Name);
                }
                else if (res.Length > 5)
                {
                    await context.PostAsync("Слишком много вариантов. Уточните.");
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    context.Call(new ClarifyDialog("Уточните город", res.Select(x => x.Name)), async (ctx, aw) =>
                      {
                          var r = await aw;
                          if (r == "")
                          {
                              await ctx.PostAsync("Введите город");
                              ctx.Wait(MessageReceivedAsync);
                          }
                          else ctx.Done(r);
                      });
                }
            }
        }
    }
}