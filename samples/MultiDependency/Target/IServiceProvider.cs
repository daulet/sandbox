namespace Samples.MultiDependency.Target
{
    public interface IServiceProvider
    {
        ProvisioningResponse Provision(ProvisioningRequest request);
    }
}
