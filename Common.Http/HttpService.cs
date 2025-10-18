using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Common.Http;

public interface IHttpService
{
    Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken cancellationToken = default);
}

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    private readonly ITraceIdAccessor _traceIdAccessor;

    public HttpService(HttpClient httpClient, ITraceIdAccessor traceIdAccessor)
    {
        _httpClient = httpClient;
        _traceIdAccessor = traceIdAccessor;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string url, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // Добавляем TraceId в заголовок каждого исходящего запроса
        request.Headers.Add("x-trace-id", _traceIdAccessor.GetValue());

        var response = await _httpClient.SendAsync(request, cancellationToken);

        // Если сервис ответил ошибкой, можно бросить исключение
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }
}