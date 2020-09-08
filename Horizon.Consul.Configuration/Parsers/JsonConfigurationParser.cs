using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Horizon.Consul.Configuration.Parsers
{
    /// <inheritdoc />
    /// <summary>
    ///     Implementation of <see cref="IConfigurationParser" /> for parsing JSON Configuration.
    /// </summary>
    public sealed class JsonConfigurationParser : IConfigurationParser
    {
        /// <inheritdoc />
        public IDictionary<string, string> Parse(Stream stream)
        {
            return new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build()
                .AsEnumerable()
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
