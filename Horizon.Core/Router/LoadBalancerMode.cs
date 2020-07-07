using System;
using System.Collections.Generic;
using System.Text;

namespace Horizon.Core.Router
{
    /// <summary>
    /// 负载均衡策略
    /// </summary>
    public enum LoadBalancerMode
    {
        Random,
        RoundRobin,
        LeastConnection,
        Hash
    }
}
