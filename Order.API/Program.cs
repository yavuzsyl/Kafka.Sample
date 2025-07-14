using Order.API.Services;
using Scalar.AspNetCore;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddSingleton<IBus, Bus>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var bus = scope.ServiceProvider.GetRequiredService<IBus>();
await bus.CreateTopic([BusConstants.OrderCreatedEventTopicName]);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
