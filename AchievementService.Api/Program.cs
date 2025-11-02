using Microsoft.EntityFrameworkCore;
using AchievementService.Dal;
using AchievementService.Logic;
using MassTransit;
using AchievementService.Api.Consumers;

var builder = WebApplication.CreateBuilder(args);

// --- Настройка твоих сервисов ---
builder.Services.AddDbContext<AchievementDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AchievementManager>();

builder.Services.AddMassTransit(x =>
{
    // 1. Регистрируем нашего "слушателя"
    x.AddConsumer<DeleteUserAchievementsConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        // 2. Адрес нашего почтового сервера RabbitMQ
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // 3. Создаем "почтовый ящик" (очередь) с именем "achievement-streak-queue"
        //    и говорим, чтобы наш слушатель (Consumer) его проверял.
        cfg.ReceiveEndpoint("delete-achievements-queue", e => 
        {
            e.ConfigureConsumer<DeleteUserAchievementsConsumer>(context);
        });
    });
});


// --- Стандартный код ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Автоматическое применение миграций ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AchievementDbContext>();
    dbContext.Database.Migrate();
}


// --- Конвейер обработки запросов ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();