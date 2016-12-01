namespace Echo.IntegrationTests.Source
{
    public class PurchaseRequest
    {
        public User Customer { get; set; }

        public PaymentInstrument Payment { get; set; }

        public ServiceType ServiceType { get; set; }
    }
}
