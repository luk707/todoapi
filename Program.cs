using Microsoft.OpenApi.Models;
using TodoApi.Data;
using TodoApi.Repositories;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations(); // 👈 Enable annotations
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ASP.NET Todo API",
        Version = "v1",
        Description = "A simple ASP.NET Core Todo API"
    });
});

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                        ?? builder.Configuration.GetConnectionString("DefaultConnection");

// Register DbContext
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// Register Repository
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
