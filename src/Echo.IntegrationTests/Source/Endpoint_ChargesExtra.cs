namespace Echo.IntegrationTests.Source
{
    internal class Endpoint_ChargesExtra : IEndpoint
    {
        private readonly IBilling _billing;
        private readonly IProvider _provider;

        public Endpoint_ChargesExtra(IBilling billing, IProvider provider)
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
                Amount = quoteResponse.Price + 1,
                Instrument = request.Payment,
                Payee = request.Customer,
            });

            if (paymentResponse.Result != PaymentCode.Success)
            {
                throw new PurchaseFailureException(paymentResponse.Message);
            }

            try
            {
                _provider.Provision(new ProvisioningRequest()
                {
                    Customer = request.Customer,
                    Service = request.ServiceType,
                });
            }
            catch (ProvisioningFailureException ex)
            {
                //refund
                var refundResponse = _billing.Charge(new PaymentRequest()
                {
                    Amount = -quoteResponse.Price,
                    Instrument = request.Payment,
                    Payee = request.Customer,
                });

                if (refundResponse.Result == PaymentCode.Success)
                {
                    throw new PurchaseFailureException("Failed to provision", ex);
                }
                else
                {
                    throw new PurchaseFailureException("Failed to provision and refund", ex);
                }
            }

            return new PurchaseResponse();
        }
    }
}
