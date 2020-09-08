using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Horizon.Consul.Configuration.Ext;
using Microsoft.Extensions.Configuration;

namespace Horizon.Consul.Configuration
{
  internal sealed class ConsulConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IConsulClientFactory _consulClientFactory;
        private readonly IConsulConfigurationSource _source;
        private ulong _lastIndex;
        private Task? _pollTask;
        private bool _disposed;

        public ConsulConfigurationProvider(
            IConsulConfigurationSource source,
            IConsulClientFactory consulClientFactory)
        {
            if (source.Parser == null)
            {
                throw new ArgumentNullException(nameof(source.Parser));
            }

            _source = source;
            _consulClientFactory = consulClientFactory;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _disposed = true;
        }

        public override void Load()
        {
            // If polling has already begun then calling load is pointless
            if (_pollTask != null)
            {
                return;
            }

            var cancellationToken = _cancellationTokenSource.Token;

            DoLoad(cancellationToken).GetAwaiter().GetResult();

            // Polling starts after the initial load to ensure no concurrent access to the key from this instance
            if (_source.ReloadOnChange)
            {
                _pollTask = Task.Run(() => PollingLoop(cancellationToken), cancellationToken);
            }
        }

        private async Task DoLoad(CancellationToken cancellationToken)
        {
            try
            {
                var result = await GetKvPairs(false, cancellationToken).ConfigureAwait(false);

                if (result.HasValue())
                {
                    SetData(result);
                }
                else if (!_source.Optional)
                {
                    throw new Exception($"The configuration for key {_source.Key} was not found and is not optional.");
                }

                SetLastIndex(result);
            }
            catch (Exception exception)
            {
                var exceptionContext = new ConsulLoadExceptionContext(_source, exception);
                _source.OnLoadException?.Invoke(exceptionContext);
                if (!exceptionContext.Ignore)
                {
                    throw;
                }
            }
        }

        private async Task<QueryResult<KVPair[]>> GetKvPairs(bool waitForChange, CancellationToken cancellationToken)
        {
            using var consulClient = _consulClientFactory.Create();
            var queryOptions = new QueryOptions
            {
                WaitTime = _source.PollWaitTime,
                WaitIndex = waitForChange ? _lastIndex : 0
            };

            var result =
                await consulClient
                    .KV
                    .List(_source.Key, queryOptions, cancellationToken)
                    .ConfigureAwait(false);

            return result.StatusCode switch
            {
                HttpStatusCode.OK => result,
                HttpStatusCode.NotFound => result,
                _ => throw new Exception($"Error loading configuration from consul. Status code: {result.StatusCode}.")
            };
        }

        private async Task PollingLoop(CancellationToken cancellationToken)
        {
            var consecutiveFailureCount = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await GetKvPairs(true, cancellationToken).ConfigureAwait(false);

                    if (result.HasValue() && result.LastIndex > _lastIndex)
                    {
                        SetData(result);
                        OnReload();
                    }

                    SetLastIndex(result);
                    consecutiveFailureCount = 0;
                }
                catch (Exception exception)
                {
                    var wait =
                        _source.OnWatchException?.Invoke(
                            new ConsulWatchExceptionContext(exception, ++consecutiveFailureCount, _source)) ??
                        TimeSpan.FromSeconds(5);
                    await Task.Delay(wait, cancellationToken);
                }
            }
        }

        private void SetData(QueryResult<KVPair[]> result)
        {
            Data = result.ToConfigDictionary(_source.ConvertConsulKVPairToConfig);
        }

        private void SetLastIndex(QueryResult result)
        {
            _lastIndex = result.LastIndex == 0
                ? 1
                : result.LastIndex < _lastIndex
                    ? 0
                    : result.LastIndex;
        }
    }
}
