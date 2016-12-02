namespace Samples.Demo.Source
{
    public class PaymentRequest
    {
        public double Amount { get; set; }

        public PaymentInstrument Instrument { get; set; }

        public User Payee { get; set; }
    }
}
