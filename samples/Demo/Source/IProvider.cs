namespace Samples.Demo.Source
{
    public interface IProvider
    {
        ProvisioningResponse Provision(ProvisioningRequest request);
    }
}
