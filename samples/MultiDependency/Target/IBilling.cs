namespace Samples.MultiDependency.Target
{
    public interface IBilling
    {
        QuoteResponse GetQuote(QuoteRequest request);

        PaymentResponse Charge(PaymentRequest request);
    }
}
