using MassTransit;
using TiendaGod.Shared.Events;

internal class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    public ILogger<UserRegisteredConsumer> _logger;
    public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var user = context.Message;
        _logger .LogInformation("New user registered: {UserId} - {Email}", user.UserId, user.Email);
        return Task.CompletedTask;
    }
}