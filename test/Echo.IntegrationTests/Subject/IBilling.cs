namespace Echo.IntegrationTests.Subject
{
    public interface IBilling
    {
        QuoteResponse GetQuote(QuoteRequest request);

        PaymentResponse Charge(PaymentRequest request);
    }
}
