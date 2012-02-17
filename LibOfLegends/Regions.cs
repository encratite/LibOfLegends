using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOfLegends
{
    public enum RegionTag
    {
        NA,
        EUW,
        EUNE
    }

    public class Regions
    {
        public static Dictionary<RegionTag, string> HostnameTags = new Dictionary<RegionTag, string>()
        {
            {RegionTag.NA, "na1"},
            {RegionTag.EUW, "eu"},
            {RegionTag.EUNE, "eune"}
        };
    }
}
