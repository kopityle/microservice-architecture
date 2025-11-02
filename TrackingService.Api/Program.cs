using Common.Http;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TrackingService.Api.Middleware;
using TrackingService.CoreLib.Interfaces;
using TrackingService.Dal;
using TrackingService.Infrastructure.Clients;
using TrackingService.Logic;

var builder = WebApplication.CreateBuilder(args);


// --- Регистрация сервисов приложения ---

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
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
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
