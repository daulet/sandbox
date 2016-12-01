namespace Echo.IntegrationTests.Source
{
    public interface IBilling
    {
        QuoteResponse GetQuote(QuoteRequest request);

        PaymentResponse Charge(PaymentRequest request);
    }
}
