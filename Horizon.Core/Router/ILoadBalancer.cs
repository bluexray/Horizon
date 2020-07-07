using Horizon.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Horizon.Core.Router
{
    public interface ILoadBalancer
    {
        Task<HostAndPort> SelectManyAsync(CancellationToken ct = default(CancellationToken));
    }
}
