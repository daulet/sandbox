namespace Samples.MultiDependency.Target
{
    public interface IEndpoint
    {
        PurchaseResponse Purchase(PurchaseRequest request);
    }
}
