using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Horizon.Core.Docker
{
 public static class DockerHelper
    {
        public static bool IsRunningInDocker
        {
            get
            {
                bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inContainer);
                return inContainer;
            }
        }

        public static string DockerId()
        {
            string hostname = Environment.GetEnvironmentVariable("HOSTNAME", EnvironmentVariableTarget.Process);
            return string.IsNullOrWhiteSpace(hostname) ? "" : hostname;
        }

        public static string? ContainerAddress
        {
            get
            {
                if (IsRunningInDocker)
                {
                    var name = Dns.GetHostName(); // get container id
                    return Dns.GetHostEntry(name).AddressList
                        .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)?.ToString();
                }

                return null;
            }
        }
    }
}
