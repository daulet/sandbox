using System;
using System.Globalization;
using Echo.Logging;

namespace Echo.Sample.RestrictionSample
{
    internal class CreditCardBilling : IBilling
    {
        private readonly ILogger _logger;

        internal CreditCardBilling(ILogger logger)
        {
            _logger = logger;
        }

        public bool Charge(string username, int amount)
        {
            _logger.Info($"Charging {username} for {amount}");

            return true;
        }

        public DateTimeOffset GetExpirationDate(string username)
        {
            _logger.Info($"Getting expiration for {username}");

            return DateTimeOffset.UtcNow.AddDays(-1);
        }

        public int GetPrice(RegionInfo region)
        {
            _logger.Info($"Obtaining price for {region.DisplayName}");

            return 100;
        }
    }
}
