using Common.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Threading.Tasks;
using TrackingService.CoreLib.Interfaces;

namespace TrackingService.Infrastructure.Clients;

public class HabitServiceApiClient : IHabitServiceClient
{
    private readonly IHttpService _httpService;
    private readonly string _baseUrl;

    public HabitServiceApiClient(IHttpService httpService, IConfiguration configuration)
    {
        _httpService = httpService;
        _baseUrl = configuration["ServiceUrls:HabitService"]!;
    }

    public async Task<bool> HabitExistsAsync(Guid habitId, CancellationToken cancellationToken)
    {
        try
        {
            // Делаем запрос на фейковый эндпоинт, который мы потом создадим
            var result = await _httpService.GetAsync<object>($"{_baseUrl}/api/habits/{habitId}", cancellationToken);
            return result != null;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Если сервис ответил "404 Not Found", значит привычки нет
            return false;
        }
    }
}