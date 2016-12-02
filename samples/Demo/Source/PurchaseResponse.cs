using System;

namespace Samples.Demo.Source
{
    public class PurchaseResponse
    {
    }

    public class PurchaseFailureException : Exception
    {
        public PurchaseFailureException(string message)
            : base(message)
        { }

        public PurchaseFailureException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
