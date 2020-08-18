using System.Collections.Generic;
using System.Linq;
using Horizon.Core.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Horizon.Core.Router
{
    internal class NoLoadBalancer : ILoadBalancer
    {
        private readonly IList<ServiceInformation> _services;

        public NoLoadBalancer(IList<ServiceInformation> services)
        {
            _services = services;
        }

        public async Task<HostAndPort> SelectManyAsync(CancellationToken ct = default)
        {
            var service = await Task.FromResult(_services.FirstOrDefault());
            return service.HostAndPort;
        }
    }
}