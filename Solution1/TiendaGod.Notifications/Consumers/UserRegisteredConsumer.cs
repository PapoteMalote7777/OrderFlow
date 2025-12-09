using MassTransit;
using TiendaGod.Shared.Events;
using TiendaGod.Notifications.Services;

internal class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredConsumer> _logger;
    private readonly EmailService _emailService;

    public UserRegisteredConsumer(
        ILogger<UserRegisteredConsumer> logger,
        EmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var user = context.Message;

        _logger.LogInformation("New user registered: {Email}", user.Email);

        await _emailService.SendAsync(
            user.Email,
            "Bienvenido a TiendaGod",
            $"""
            <h1>Bienvenido {user.UserName}</h1>
            <p>Tu cuenta fue creada correctamente.</p>
            <b>Disfruta de TiendaGod</b>
            """
        );
    }
}
