namespace Samples.MultiDependency.Target
{
    public interface IProvider
    {
        ProvisioningResponse Provision(ProvisioningRequest request);
    }
}
