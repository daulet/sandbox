namespace Echo.IntegrationTests.Subject
{
    public interface IEndpoint
    {
        PurchaseResponse Purchase(PurchaseRequest request);
    }
}
