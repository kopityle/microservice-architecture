using Common.Http; // Наш ITraceIdAccessor

namespace TrackingService.Api.Middleware;

public class TraceIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string TraceIdHeaderName = "x-trace-id";

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITraceIdAccessor traceIdAccessor)
    {
        // Пытаемся прочитать TraceId из заголовка входящего запроса
        var traceId = context.Request.Headers[TraceIdHeaderName].FirstOrDefault();

        // Записываем его в наш сервис. 
        // Если traceId был пустой, TraceIdAccessor сам сгенерирует новый.
        traceIdAccessor.WriteValue(traceId);

        // Добавляем TraceId в заголовок ответа, чтобы его мог увидеть клиент
        context.Response.OnStarting(() =>
        {
            if (!context.Response.Headers.ContainsKey(TraceIdHeaderName))
            {
                context.Response.Headers.Add(TraceIdHeaderName, traceIdAccessor.GetValue());
            }
            return Task.CompletedTask;
        });

        // Передаем запрос дальше по цепочке
        await _next(context);
    }
}