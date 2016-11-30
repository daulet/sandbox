namespace Samples.MultiDependency.Target
{
    public class PurchaseRequest
    {
        public User Customer { get; set; }

        public PaymentInstrument Payment { get; set; }

        public ServiceType ServiceType { get; set; }
    }
}
