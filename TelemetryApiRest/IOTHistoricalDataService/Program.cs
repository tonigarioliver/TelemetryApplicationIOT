// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using IOTHistoricalDataService.Services;
using IOTHistoricalDataService.EventProcessing;
using IOTHistoricalDataService.Config;
using IOTHistoricalDataService.Data;
using IOTHistoricalDataService.COR;
using IOTHistoricalDataService.Services.Implementation;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        Console.WriteLine($"Running in the {environmentName} environment.");

        MyBackgroundTask(environmentName).Wait();
    }


    public static async Task MyBackgroundTask(string environmentName)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .Build();
        var services = new ServiceCollection();

        // Configure MQTTSettings
        services.Configure<MQTTSettings>(configuration.GetSection("MQTTSettings"));
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQSettings"));

        // Configure and add the DbContext
        var connection = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<appDbContext>(options =>
            options.UseNpgsql(connection)
        );
        Console.WriteLine(connection);
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.AddConsole();
        });

        // Add AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Add your custom services
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddSingleton<IEventProcessor, EventProcessor>();
        services.AddSingleton<NewDataMQTTProcess>();
        services.AddSingleton<RabbitMQManager>();

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();
 
        using (var scope = serviceProvider.CreateScope())
        {
            var mqttService = scope.ServiceProvider.GetRequiredService<NewDataMQTTProcess>();
            var rabbitMQManager = scope.ServiceProvider.GetRequiredService<RabbitMQManager>();
            var deviceService = scope.ServiceProvider.GetRequiredService<IDeviceService>();
            var devices = await deviceService.GetAllDevices();
            foreach (var device in devices)
                Console.WriteLine(device.SerialNumber);
            // Suscribirse a mensajes
            var message = Encoding.UTF8.GetBytes("Mensaje de prueba");
            rabbitMQManager.Subscribe("mi_cola", message => Console.WriteLine(Encoding.UTF8.GetString(message)));
            rabbitMQManager.PublishMessage(exchange: "", routingKey: "mi_cola", messageBody: message);

            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            await mqttService.ExecuteAsync(cancellationToken);
            // Mantén la aplicación en ejecución
            rabbitMQManager.PublishMessage(exchange: "", routingKey: "mi_cola", messageBody: message);
            while (true) ;
        }
    }
}