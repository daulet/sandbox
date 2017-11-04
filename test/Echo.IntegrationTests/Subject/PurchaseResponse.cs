using System;

namespace Echo.IntegrationTests.Subject
{
    public class PurchaseResponse
    {
    }

    public class PurchaseFailureException : Exception
    {
        // @TODO remove this constructor and tests start failing
        // make the deserializer more resilient to missing default constructors
        public PurchaseFailureException()
        { }

        public PurchaseFailureException(string message)
            : base(message)
        { }

        public PurchaseFailureException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
