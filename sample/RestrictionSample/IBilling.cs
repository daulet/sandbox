using System;
using System.Globalization;
using Echo.Restriction;

namespace Echo.Sample.RestrictionSample
{
    public interface IBilling
    {
        [Restricted]
        bool Charge(string username, int amount);

        DateTimeOffset GetExpirationDate(string username);

        int GetPrice(RegionInfo region);
    }
}
