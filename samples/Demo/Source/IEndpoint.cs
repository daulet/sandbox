namespace Samples.Demo.Source
{
    public interface IEndpoint
    {
        PurchaseResponse Purchase(PurchaseRequest request);
    }
}
