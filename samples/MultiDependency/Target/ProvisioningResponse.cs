using System;
using System.Collections.Generic;

namespace Samples.MultiDependency.Target
{
    public class ProvisioningResponse
    {
        public IEnumerable<ServiceType> ProvisionedServices { get; set; }
    }

    public class ProvisioningFailureException : Exception
    {

    }
}
