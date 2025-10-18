using Microsoft.Extensions.DependencyInjection;

namespace Common.Http;

public static class HttpServiceExtensions
{
    public static IServiceCollection AddHttpService(this IServiceCollection services)
    {
        services.AddScoped<ITraceIdAccessor, TraceIdAccessor>();
        services.AddHttpClient<IHttpService, HttpService>();
        return services;
    }
}