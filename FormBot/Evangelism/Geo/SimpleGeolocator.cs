using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace FormBot.Evangelism.Geo
{
    public static class SimpleGeolocator
    {
        public static Dictionary<string, string> ReverseLookup(double lat, double lng)
        {
            var doc = new XmlDocument();
            doc.Load("http://maps.googleapis.com/maps/api/geocode/xml?latlng=" + lat + "," + lng + "&sensor=false");
            XmlNode element = doc.SelectSingleNode("//GeocodeResponse/status");
            if (element.InnerText == "ZERO_RESULTS")
            {
                return null;
            }
            else
            {
                var R = new Dictionary<string, string>();
                element = doc.SelectSingleNode("//GeocodeResponse/result/formatted_address");
                var xnList = doc.SelectNodes("//GeocodeResponse/result/address_component");
                foreach (XmlNode xn in xnList)
                {
                    R.Add(xn["type"].InnerText, xn["short_name"].InnerText);
                }
                return R;
            }
        }
        }
}