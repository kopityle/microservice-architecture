using HabitService.Application.Interfaces;
using HabitService.Application.Services;
using HabitService.Domain.Interfaces;
using HabitService.Infrastructure.Persistence;
using HabitService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- ÍÀÑÒĞÎÉÊÀ ÑÅĞÂÈÑÎÂ ---

// 1. Ğåãèñòğèğóåì DbContext
builder.Services.AddDbContext<HabitDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Ğåãèñòğèğóåì íàøè ğåïîçèòîğèè (Infrastructure ñëîé)
builder.Services.AddScoped<IHabitRepository, HabitRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// 3. Ğåãèñòğèğóåì íàøè ñåğâèñû (Application ñëîé)
builder.Services.AddScoped<IHabitManagementService, HabitManagementService>();


// --- ÑÒÀÍÄÀĞÒÍÛÉ ÊÎÄ ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- ÑÁÎĞÊÀ ÏĞÈËÎÆÅÍÈß ---
var app = builder.Build();

// --- ÀÂÒÎÌÀÒÈ×ÅÑÊÎÅ ÏĞÈÌÅÍÅÍÈÅ ÌÈÃĞÀÖÈÉ ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HabitDbContext>();
    dbContext.Database.Migrate();
}
// ---------------------------------------------


// --- ÍÀÑÒĞÎÉÊÀ ÊÎÍÂÅÉÅĞÀ ÎÁĞÀÁÎÒÊÈ ÇÀÏĞÎÑÎÂ ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// --- ÇÀÏÓÑÊ ÏĞÈËÎÆÅÍÈß ---
app.Run();