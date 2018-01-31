using GeoCoordinatePortable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormBot.Evangelism.Geo
{
    [Serializable]
    public class GeoCity
    {
        public string Name { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
        public string Country { get; set; }
        public double Dist(double Lat, double Long)
        {
            var gc1 = new GeoCoordinate(Lat,Long);
            var gc2 = new GeoCoordinate(this.Lat, this.Long);
            return gc1.GetDistanceTo(gc2);
        }

        public bool NameSimilarTo(string name)
        {
            name = name.ToLower();
            var name1 = Name.ToLower();
            return (name == name1) || name.Contains(name1) || name1.Contains(name);
        }

        public static GeoCity[] GetClosestCities(GeoCity[] Cities, double Lat, double Long, int max = 5, double variance = 0.2)
        {
            var res = (from z in Cities
                      let d = z.Dist(Lat, Long)
                      orderby d
                      select new Tuple<GeoCity,double>(z,d)).ToArray();
            var r = new List<GeoCity>();
            r.Add(res[0].Item1);
            for (int i=1;i<max;i++)
            {
                if (res[1].Item2 < res[0].Item2 * (1 + variance)) r.Add(res[i].Item1);
                else break;
            }
            return r.ToArray();
        }

        public static GeoCity[] GetSimilarCities(GeoCity[] Cities, string name)
        {
            return (from z in Cities
                    where z.NameSimilarTo(name)
                    select z).ToArray();
        }

    }
}