using MassTransit;
using TiendaGod.Notifications.Services;
using TiendaGod.Shared.Events;

internal class UserDeletedConsumer : IConsumer<UserDeletedEvent>
{
    private readonly ILogger<UserDeletedConsumer> _logger;
    private readonly EmailService _emailService;

    public UserDeletedConsumer(
        ILogger<UserDeletedConsumer> logger,
        EmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UserDeletedEvent> context)
    {
        var user = context.Message;

        _logger.LogInformation("User deleted: {Email}", user.Email);

        await _emailService.SendAsync(
            user.Email,
            "Cuenta eliminada",
            "<p>Tu cuenta fue eliminada correctamente.</p>"
        );
    }
}