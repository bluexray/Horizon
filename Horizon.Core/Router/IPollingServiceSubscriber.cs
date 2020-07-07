using System;
using System.Threading;
using System.Threading.Tasks;

namespace Horizon.Core.Router
{
    public interface IPollingServiceSubscriber:IServiceSubscriber
    {
        Task StartSubscription(CancellationToken ct = default(CancellationToken));

        event EventHandler EndpointsChanged;
    }
}