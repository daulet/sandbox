using System;
using System.Globalization;

namespace Echo.Sample.RestrictionSample
{
    public class SubscriptionImplementation
    {
        private readonly IBilling _billing;

        public SubscriptionImplementation(IBilling billing)
        {
            _billing = billing;
        }

        /// <returns>Whether user is still subscribed</returns>
        public bool Renew(string username, RegionInfo region)
        {
            var expirationDate = _billing.GetExpirationDate(username);

            if (expirationDate <= DateTimeOffset.UtcNow)
            {
                var price = _billing.GetPrice(region);
                return _billing.Charge(username, price);
            }

            return true;
        }
    }
}
