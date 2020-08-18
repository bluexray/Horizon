using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Horizon.Core.Model;

namespace Horizon.Core.Router
{
    public class LoadBalancerFactory
    {

        private static IList<ServiceInformation> _service;
        public LoadBalancerFactory(IList<ServiceInformation> service)
        {
            _service = service;
        }

        public async Task<ILoadBalancer> Get(string serviceName, LoadBalancerMode loadBalancer = LoadBalancerMode.RoundRobin)
        {
            switch (loadBalancer)
            {
                case LoadBalancerMode.Random:
                    return new RandomLoadBalancer(_service, serviceName);
                case LoadBalancerMode.RoundRobin:
                    return new RoundRobinLoadBalancer(_service, serviceName);
                default:
                    return new NoLoadBalancer(_service);
            }
        }
    }
}
