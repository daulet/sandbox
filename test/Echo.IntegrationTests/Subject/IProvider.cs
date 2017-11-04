namespace Echo.IntegrationTests.Subject
{
    public interface IProvider
    {
        ProvisioningResponse Provision(ProvisioningRequest request);
    }
}
