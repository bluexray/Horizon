using System;
using System.Collections.Generic;
using System.Text;
using Consul;

namespace Horizon.Consul.Configuration
{
    interface IConsulClientFactory
    {
        public IConsulClient Create();
    }
}
