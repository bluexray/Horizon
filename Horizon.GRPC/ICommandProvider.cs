using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace Horizon.GRPC.Interceptors
{
    public interface ICommandProvider
    {
        bool LimitMaxRequest(ServerCallContext context);

        T ExecuteTimeout<T>(ServerCallContext context, Func<Task<T>> func);

        Task<T> BreakerRequestCircuitBreaker<T>(ServerCallContext context, Func<Task<T>> func);
    }
}