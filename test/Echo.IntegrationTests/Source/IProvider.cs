namespace Echo.IntegrationTests.Source
{
    public interface IProvider
    {
        ProvisioningResponse Provision(ProvisioningRequest request);
    }
}
