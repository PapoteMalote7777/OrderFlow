using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using TiendaGod.Notifications.Services;
using TiendaGod.Shared.Events;

internal class OrderAcceptedConsumer : IConsumer<OrderAcceptedEvent>
{
    private readonly ILogger<OrderAcceptedConsumer> _logger;
    private readonly EmailService _emailService;
    private readonly HttpClient _httpClient;

    public OrderAcceptedConsumer(
        ILogger<OrderAcceptedConsumer> logger,
        EmailService emailService,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _emailService = emailService;
        _httpClient = httpClientFactory.CreateClient("Identity");
    }

    public async Task Consume(ConsumeContext<OrderAcceptedEvent> context)
    {
        var pedido = context.Message;

        _logger.LogInformation("Pedido aceptado: {PedidoId}", pedido.PedidoId);

        var emailResponse = await _httpClient
            .GetFromJsonAsync<UserEmailDto>($"/api/User/email/{pedido.UserId}");

        if (emailResponse == null)
        {
            _logger.LogError("No se pudo obtener el email del usuario {UserId}", pedido.UserId);
            return;
        }

        await _emailService.SendAsync(
            emailResponse.Email,
            "Pedido aceptado",
            $"""
            <h2>Pedido #{pedido.PedidoId} aceptado</h2>
            <p>Tu pedido ha sido aceptado y se encuentra en proceso de envío.</p>
            <p>Revisa tus pedidos en la web para más información.</p>
            """
        );
    }
}
