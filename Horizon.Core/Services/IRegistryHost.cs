using System;
using System.Collections.Generic;
using System.Text;

namespace Horizon.Core.Services
{
    public interface IRegistryHost:IHaveKeyValues,IResolveServiceInstances,IServiceHealthCheck,IServiceInstances
    {
    }
}
