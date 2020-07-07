using System;
using System.Collections.Generic;
using System.Text;

namespace Horizon.Consul.Configurations
{
    public class ConsulServiceDiscoveryOption
    {

        public string ServiceName { get; set; }

        public string Version { get; set; }

        public ConsulHostConfiguration Consul { get; set; }

        public string HealthCheckTemplate { get; set; }

        public string[] Endpoints { get; set; }
    }
}
