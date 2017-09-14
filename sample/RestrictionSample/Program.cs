using System;
using System.Globalization;
using Echo.Restriction;

namespace Echo.Sample.RestrictionSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // provide your own logging adapter
            var logger = new Logger();
            // create instance of RestrictingProvider
            var provider = new RestrictingProvider(logger);
            // get instance of the real dependency
            var resource = new CreditCardBilling(logger);
            // restrict methods that you don't want call in dry run
            var restrictedResource = provider.GetRestrictedTarget<IBilling>(resource);
            // inject restricted dependency to the implementation under test
            var implementation = new SubscriptionImplementation(restrictedResource);
            // dry run your implementation
            implementation.Renew("cartman", RegionInfo.CurrentRegion);

            Console.ReadKey();
        }
    }
}
