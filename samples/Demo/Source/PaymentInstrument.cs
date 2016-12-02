using System;

namespace Samples.Demo.Source
{
    public abstract class PaymentInstrument
    {
    }

    public class PayPalPaymentInstrument : PaymentInstrument
    {
        public string AuthorizationToken { get; set; }
    }

    public class CreditCardPaymentInstrument : PaymentInstrument
    {
        public DateTime CardExpirationDate { get; set; }

        public long CardNumber { get; set; }

        public string CardOwner { get; set; }

        public CreditCardProvider CardProvider { get; set; }
    }

    public enum CreditCardProvider
    {
        Discover,
        MasterCard,
        Visa,
    }
}
