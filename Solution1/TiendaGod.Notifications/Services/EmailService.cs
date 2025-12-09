using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TiendaGod.Notifications.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("Email");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(emailSettings["From"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = body
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                emailSettings["Host"],
                int.Parse(emailSettings["Port"]!),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                emailSettings["Username"],
                emailSettings["Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
