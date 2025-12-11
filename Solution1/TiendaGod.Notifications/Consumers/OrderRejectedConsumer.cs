using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using TiendaGod.Notifications.Services;
using TiendaGod.Shared.Events;

internal class OrderRejectedConsumer : IConsumer<OrderRejectedEvent>
{
    private readonly ILogger<OrderRejectedConsumer> _logger;
    private readonly EmailService _emailService;
    private readonly HttpClient _httpClient;

    public OrderRejectedConsumer(
        ILogger<OrderRejectedConsumer> logger,
        EmailService emailService,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _emailService = emailService;
        _httpClient = httpClientFactory.CreateClient("Identity");
    }

    public async Task Consume(ConsumeContext<OrderRejectedEvent> context)
    {
        var pedido = context.Message;

        _logger.LogInformation("Pedido rechazado: {PedidoId}", pedido.PedidoId);

        var emailResponse = await _httpClient
            .GetFromJsonAsync<UserEmailDto>($"/api/User/email/{pedido.UserId}");

        if (emailResponse == null)
        {
            _logger.LogError("No se pudo obtener el email del usuario {UserId}", pedido.UserId);
            return;
        }

        await _emailService.SendAsync(
            emailResponse.Email,
            "Pedido rechazado",
            $"""
            <h2>Pedido #{pedido.PedidoId} rechazado</h2>
            <p>Tu pedido ha sido rechazado. Por favor, ponte en contacto con el administrador para más información.</p>
            """
        );
    }
}
