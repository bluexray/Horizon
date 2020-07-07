using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Horizon.Core.Router
{
    public interface ILoadBalancerManager
    {
        Task<ILoadBalancer> Get(string serviceName, LoadBalancerMode loadBalancer = LoadBalancerMode.RoundRobin);
    }
}
