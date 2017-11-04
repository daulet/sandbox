using Echo.IntegrationTests.Subject;

namespace Echo.IntegrationTests.UnitTesting
{
    internal class Endpoint_NoProvision : IEndpoint
    {
        private readonly IBilling _billing;
        private readonly IProvider _provider;

        public Endpoint_NoProvision(IBilling billing, IProvider provider)
        {
            _billing = billing;
            _provider = provider;
        }

        public PurchaseResponse Purchase(PurchaseRequest request)
        {
            var quoteResponse = _billing.GetQuote(new QuoteRequest()
            {
                Service = request.ServiceType,
            });
            var paymentResponse = _billing.Charge(new PaymentRequest()
            {
                Amount = quoteResponse.Price,
                Instrument = request.Payment,
                Payee = request.Customer,
            });

            if (paymentResponse.Result != PaymentCode.Success)
            {
                throw new PurchaseFailureException(paymentResponse.Message);
            }

            // THIS IS THE BUG - No Provision call

            return new PurchaseResponse();
        }
    }
}
