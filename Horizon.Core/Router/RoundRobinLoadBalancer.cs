using Horizon.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Horizon.Core.Router
{
    /// <summary>
    /// 主机地址的轮播负载均衡策略
    /// </summary>
    public class RoundRobinLoadBalancer:ILoadBalancer
    {
        private readonly IList<ServiceInformation> _subscriber;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private int _index;
        private readonly string _serviceName;

        public RoundRobinLoadBalancer(IList<ServiceInformation> subscriber,string servicename)
        {
            _subscriber = subscriber;
            _serviceName = servicename;
        }

        public async Task<ServiceInformation> Endpoint(CancellationToken ct = default(CancellationToken))
        {
            var endpoints =  _subscriber;
            if (endpoints.Count == 0)
            {
                throw new ArgumentNullException($"{_serviceName}");
            }

            if (endpoints.Count==1)
            {
                return endpoints[0];
            }


            await _lock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_index >= endpoints.Count)
                {
                    _index = 0;
                }
                var uri = endpoints[_index];
                _index++;

                return uri;
            }
            finally
            {
                _lock.Release();
            }
        }

        public Task<HostAndPort> SelectManyAsync(CancellationToken ct = default)
        {
            return Task.FromResult(Endpoint().Result.HostAndPort);
        }
    }
}
