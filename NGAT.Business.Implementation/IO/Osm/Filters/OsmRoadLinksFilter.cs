using System;
using System.Collections.Generic;
using System.Text;

namespace NGAT.Services.IO.Osm.Filters
{
    public class OsmRoadLinksFilterCollection : IO.Filters.LinkFilterCollection
    {
        public OsmRoadLinksFilterCollection()
        {
            this.Add(attrs =>
            {
                return attrs.ContainsKey("highway")
                && (attrs["highway"].ToLowerInvariant() != "pedestrian"
                && attrs["highway"].ToLowerInvariant() != "footway"
                && attrs["highway"].ToLowerInvariant() != "steps"
                && attrs["highway"].ToLowerInvariant() != "service");
            });
        }
    }
}
