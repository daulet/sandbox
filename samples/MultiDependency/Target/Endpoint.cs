namespace Samples.MultiDependency.Target
{
    internal class Endpoint : IEndpoint
    {
        private readonly IBilling _billing;
        private readonly IServiceProvider _serviceProvider;

        public Endpoint(IBilling billing, IServiceProvider serviceProvider)
        {
            _billing = billing;
            _serviceProvider = serviceProvider;
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

            try
            {
                _serviceProvider.Provision(new ProvisioningRequest()
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
