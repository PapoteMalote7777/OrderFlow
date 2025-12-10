using MassTransit;
using System.Net.Http;
using System.Net.Http.Json;
using TiendaGod.Notifications.Services;
using TiendaGod.Shared.Events;

internal class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly EmailService _emailService;
    private readonly HttpClient _httpClient;

    public OrderCreatedConsumer(
        ILogger<OrderCreatedConsumer> logger,
        EmailService emailService,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _emailService = emailService;
        _httpClient = httpClientFactory.CreateClient("Identity");
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var pedido = context.Message;

        _logger.LogInformation("Order created: {PedidoId}", pedido.PedidoId);

        var emailResponse = await _httpClient
            .GetFromJsonAsync<UserEmailDto>($"/api/User/email/{pedido.UserId}");

        if (emailResponse == null)
        {
            _logger.LogError("No se pudo obtener el email del usuario {UserId}", pedido.UserId);
            return;
        }

        await _emailService.SendAsync(
            emailResponse.Email,
            "Pedido confirmado",
            $"""
            <h2>Pedido #{pedido.PedidoId}</h2>
            <p>Total: <b>{pedido.Total}€</b></p>
            <p>Gracias por tu compra.</p>
            <p>Revise mis pedidos en la web para más información.</p>
            """
        );
    }
}

public record UserEmailDto(string Email);
