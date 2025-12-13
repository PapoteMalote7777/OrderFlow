using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using TiendaGod.Notifications.Services;
using TiendaGod.Shared.Events;

internal class AdminNewOrderConsumer : IConsumer<AdminNewOrderEvent>
{
    private readonly ILogger<AdminNewOrderConsumer> _logger;
    private readonly EmailService _emailService;
    private readonly HttpClient _httpClient;

    public AdminNewOrderConsumer(
        ILogger<AdminNewOrderConsumer> logger,
        EmailService emailService,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _emailService = emailService;
        _httpClient = httpClientFactory.CreateClient("Identity");
    }

    public async Task Consume(ConsumeContext<AdminNewOrderEvent> context)
    {
        var pedido = context.Message;

        _logger.LogInformation("Nuevo pedido: {PedidoId}", pedido.PedidoId);

        var adminEmails = await _httpClient
            .GetFromJsonAsync<List<UserEmailDto>>("/api/UserAdmin/admin-emails");

        if (adminEmails == null || !adminEmails.Any())
        {
            _logger.LogError("No se pudieron obtener los emails de los administradores");
            return;
        }

        foreach (var admin in adminEmails)
        {
            await _emailService.SendAsync(
                admin.Email,
                "Nuevo pedido recibido",
                $"""
                <h2>Nuevo pedido #{pedido.PedidoId}</h2>
                <p>Revisa la sección de notificaciones para aceptarlo o rechazarlo.</p>
                """
            );
        }
    }
}
