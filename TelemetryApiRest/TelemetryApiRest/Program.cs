using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TelemetryApiRest.Config;
using TelemetryApiRest.COR;
using TelemetryApiRest.Data;
using TelemetryApiRest.EventProcessing;
using TelemetryApiRest.Helper;
using TelemetryApiRest.Services;
using TelemetryApiRest.Services.Implementation;
using TelemetryApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MQTTSettings>(builder.Configuration.GetSection("MQTTSettings"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQSettings"));

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TelemetryApiDbContext>(
    options =>
    options.UseNpgsql(connection)
); //

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDeviceService,DeviceService>();
builder.Services.AddSingleton<IMqttListenerServiceRealTime, MqttListenerServiceRealTime>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddSingleton<RabbitMQManager>();
builder.Services.AddHostedService<MqttBackgroundService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<DeviceMessageHub>("deviceMessage");
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
