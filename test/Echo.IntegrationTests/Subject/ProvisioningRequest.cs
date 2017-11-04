namespace Echo.IntegrationTests.Subject
{
    public class ProvisioningRequest
    {
        public User Customer { get; set; }

        public ServiceType Service { get; set; }
    }
}
