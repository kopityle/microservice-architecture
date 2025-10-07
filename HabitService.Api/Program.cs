using HabitService.Application.Interfaces;
using HabitService.Application.Services;
using HabitService.Domain.Interfaces;
using HabitService.Infrastructure.Persistence;
using HabitService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- ��������� �������� ---

// 1. ������������ DbContext
builder.Services.AddDbContext<HabitDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. ������������ ���� ����������� (Infrastructure ����)
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// 3. ������������ ���� ������� (Application ����)
builder.Services.AddScoped<IHabitManagementService, HabitManagementService>();


// --- ����������� ��� ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- ������ ���������� ---
var app = builder.Build();

// --- �������������� ���������� �������� ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HabitDbContext>();
    dbContext.Database.Migrate();
}
// ---------------------------------------------


// --- ��������� ��������� ��������� �������� ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// --- ������ ���������� ---
app.Run();