using Common.Http;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TrackingService.Api.Middleware;
using TrackingService.CoreLib.Interfaces;
using TrackingService.Dal;
using TrackingService.Infrastructure.Clients;
using TrackingService.Logic;
using TrackingService.Api.Consumers;
using StackExchange.Redis;
using Common.Interfaces;
using TrackingService.Dal.DistributedLocks;

var builder = WebApplication.CreateBuilder(args);


// --- Регистрация сервисов приложения ---
// Подключаемся к Redis и регистрируем его как Singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"))
);

// Регистрируем нашу реализацию семафора как Singleton
builder.Services.AddSingleton<IDistributedSemaphore>(provider =>
    new RedisDistributedSemaphore(
        "mark-habit-semaphore", // Уникальное имя нашего "турникета"
        3, // Максимум 3 одновременных запроса
        provider.GetRequiredService<IConnectionMultiplexer>()
    )
);
// Слой Logic
builder.Services.AddScoped<TrackingManager>();

// Слой Infrastructure/Dal
builder.Services.AddDbContext<TrackingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IHabitServiceClient, HabitServiceApiClient>(); // Регистрация нашего HTTP-клиента

// Общие библиотеки (Libs)
builder.Services.AddHttpService();

// Настройка MassTransit с RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<DeleteUserTrackingDataConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // 2. ДОБАВЛЯЕМ НОВУЮ ОЧЕРЕДЬ И "ПРИВЯЗЫВАЕМ" К НЕЙ CONSUMER
        cfg.ReceiveEndpoint("delete-tracking-queue", e =>
        {
            e.ConfigureConsumer<DeleteUserTrackingDataConsumer>(context);
        });
    });
});

// --- Стандартная настройка ASP.NET Core ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<TraceIdMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
