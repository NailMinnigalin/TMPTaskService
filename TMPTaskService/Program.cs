using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TMPTaskService.Data.Implementations;
using TMPTaskService.Data.Interfaces;
using TMPTaskService.Domain.Implementations;
using TMPTaskService.Domain.Interfaces;
using TMPTaskService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ITaskManager, TaskManager>();
builder.Services.AddScoped<ITaskRepository, DbTaskRepository>();

if (builder.Environment.IsEnvironment("IntegrationTest"))
	builder.Services.AddDbContext<TMPDbContext>(options => options.UseInMemoryDatabase("TestDb"));
else
	builder.Services.AddDbContext<TMPDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
   

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program { }