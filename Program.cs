using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using MonitoringAndCommunication.Data;
using MonitoringAndCommunication.Services;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<RabbitMQConsumerService>();
builder.Services.AddHostedService<RabbitMQDeviceConsumerService>();
builder.Services.AddScoped<IConsumptionService, ConsumptionService>();
builder.Services.AddScoped<IDeviceConsumptionService, DeviceConsumptionService>();
builder.Services.AddSingleton<SignalRHub>();
builder.Services.AddDbContext<EnergyDataContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddSignalR();



builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:4200") 
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Allow credentials
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/*
app.UseWebSockets();*/

/*app.Use(async (context, next) =>
{
    if (context.Request.Path == "/{userId}")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            if (context.Request.Path.Value != null)
            {
                int userId = int.Parse(context.Request.Path.Value.TrimStart('/'));
                await new WebSocketManagerService().OnConnected(webSocket, userId);
            }
        }
        else
        {
            context.Response.StatusCode = 400; 
        }
    }
    else
    {
        await next(); 
    }
});*/


app.MapHub<SignalRHub>("/chathub");
app.UseAuthorization();

app.MapControllers();

app.UseCors("CorsPolicy");

app.Run();




