using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Horizon.Core.DependencyInjection
{
    public interface IHorizonBulider
    {

        IServiceCollection Services { get; }
        IConfiguration Configuration { get; }
    }
}
