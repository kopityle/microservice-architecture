using Common.Http;
using Microsoft.EntityFrameworkCore;
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


// --- Стандартная настройка ASP.NET Core ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
