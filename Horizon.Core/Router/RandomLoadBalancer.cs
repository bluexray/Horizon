using Horizon.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Horizon.Core.Router
{
    public class RandomLoadBalancer : ILoadBalancer
    {

        private readonly IList<ServiceInformation> _services;
        private readonly string _serviceName;
        private readonly Func<int, int, int> _generate;

        public RandomLoadBalancer(IList<ServiceInformation> services, string serviceName)
        {
            _services = services;
            _serviceName = serviceName;
            var random = new Random();
            _generate = (min, max) => random.Next(min, max);
        }

        public async Task<HostAndPort> SelectManyAsync(CancellationToken ct = default)
        {
            var services = _services;

            if (services == null)
                throw new ArgumentNullException($"{_serviceName}");

            if (!services.Any())
                throw new ArgumentNullException($"{_serviceName}");

            var index = _generate(0, services.Count());

            return await Task.FromResult(services[index].HostAndPort);
        }
    }
}
