using Horizon.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Horizon.Core.Router
{
    public interface IServiceSubscriber : IDisposable
    {
        Task<List<ServiceInformation>> Endpoints(CancellationToken ct = default(CancellationToken));
    }
}
