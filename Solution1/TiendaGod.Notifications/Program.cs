using MassTransit;
using TiendaGod.Notifications;
using TiendaGod.Notifications.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<UserDeletedConsumer>();
    x.AddConsumer<AdminNewOrderConsumer>();
    x.AddConsumer<OrderAcceptedConsumer>();
    x.AddConsumer<OrderRejectedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("rabbitmq");

        if (!string.IsNullOrEmpty(connectionString))
        {
            cfg.Host(new Uri(connectionString));
        }
        cfg.UseMessageRetry(r => r.Intervals(
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(30)));
        cfg.ConfigureEndpoints(context);
    });

});

builder.Services.AddHttpClient("Identity", client =>
{
    client.BaseAddress = new Uri("https://localhost:7134");
});
builder.Services.AddScoped<EmailService>();
var host = builder.Build();
host.Run();
