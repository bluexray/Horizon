using Horizon.Core.Model;
using System.Threading;
using System.Threading.Tasks;

namespace Horizon.Core.Router
{
    internal class NoLoadBalancer : ILoadBalancer
    {
        private object p;

        public NoLoadBalancer(object p)
        {
            this.p = p;
        }

        public Task<HostAndPort> SelectManyAsync(CancellationToken ct = default)
        {
            throw new System.NotImplementedException();
        }
    }
}