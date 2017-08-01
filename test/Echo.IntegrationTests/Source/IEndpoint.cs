namespace Echo.IntegrationTests.Source
{
    public interface IEndpoint
    {
        PurchaseResponse Purchase(PurchaseRequest request);
    }
}
