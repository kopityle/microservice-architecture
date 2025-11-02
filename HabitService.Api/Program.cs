using HabitService.Application.Interfaces;
using HabitService.Application.Services;
using HabitService.Domain.Interfaces;
using HabitService.Infrastructure.Persistence;
using HabitService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using HabitService.Infrastructure.Consumers;

var builder = WebApplication.CreateBuilder(args);

// --- НАСТРОЙКА СЕРВИСОВ ---

// 1. Регистрируем DbContext
builder.Services.AddDbContext<HabitDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Регистрируем наши репозитории (Infrastructure слой)
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// 3. Регистрируем наши сервисы (Application слой)
builder.Services.AddScoped<IHabitManagementService, HabitManagementService>();

// 4. --- НАСТРОЙКА MASSTRANSIT (НОВЫЙ БЛОК) ---
builder.Services.AddMassTransit(x =>
{
    // Регистрируем наш Consumer для удаления данных
    x.AddConsumer<DeleteUserHabitsConsumer>();

    // Регистрируем наш Оркестратор (State Machine)
    x.AddSagaStateMachine<UserDeletionStateMachine, UserDeletionState>()
        .EntityFrameworkRepository(r =>
        {
            r.AddDbContext<DbContext, HabitDbContext>();
            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // Создаем очередь для команды, которую слушает наш Consumer
        cfg.ReceiveEndpoint("delete-habits-queue", e =>
        {
            e.ConfigureConsumer<DeleteUserHabitsConsumer>(context);
        });

        // Настраиваем "почтовый ящик" для самого Оркестратора
        cfg.ConfigureEndpoints(context);
    });
});
// --- КОНЕЦ НОВОГО БЛОКА ---

// --- СТАНДАРТНЫЙ КОД ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- СБОРКА ПРИЛОЖЕНИЯ ---
var app = builder.Build();

// --- АВТОМАТИЧЕСКОЕ ПРИМЕНЕНИЕ МИГРАЦИЙ ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HabitDbContext>();
    dbContext.Database.Migrate();
}
// ---------------------------------------------


// --- НАСТРОЙКА КОНВЕЙЕРА ОБРАБОТКИ ЗАПРОСОВ ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// --- ЗАПУСК ПРИЛОЖЕНИЯ ---
app.Run();