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
builder.Services.AddHttpClient<IAuthenticationManager, WebAuthenticationManager>(client =>
{
	client.BaseAddress = new Uri(builder.Configuration.GetConnectionString("AuthenticationService"));
});

if (builder.Environment.IsEnvironment("IntegrationTest"))
	builder.Services.AddDbContext<TMPDbContext>(options => options.UseInMemoryDatabase("TestDb"));
else
	builder.Services.AddDbContext<TMPDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
   
var app = builder.Build();

if (builder.Configuration.GetValue<bool>("DataBaseInit"))
{
	using var scope1 = app.Services.CreateScope();
	var db1 = scope1.ServiceProvider.GetRequiredService<TMPDbContext>();
	db1.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseMiddleware<AuthorizationMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }