using Grpc.Core;
using Horizon.Core.Extensions;
using Jaeger;
using OpenTracing.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.GRPC.Interceptors
{
    public class JaegerTracingInterceptor: ServerInterceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var header = context.RequestHeaders.Where(p => p.Key == "jaeger").FirstOrDefault();
            var spanBuilder = GlobalTracer.Instance.BuildSpan(context.Method).WithTag("Request", request?.ToJson() ?? "");
            if (header != null)
            {
                var spanContext = SpanContext.ContextFromString(header.Value);
                spanBuilder = spanBuilder.AsChildOf(spanContext);
            }
            using (var scope = spanBuilder.StartActive(true))
            {
                try
                {
                    return await continuation(request, context);
                }
                catch (Exception ex)
                {
                    scope.Span.SetTag("Error", ex.ToString());
                    throw;
                }
            }
        }

    }

}
