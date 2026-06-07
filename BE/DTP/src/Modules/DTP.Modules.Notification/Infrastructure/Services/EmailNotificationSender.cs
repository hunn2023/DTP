using DTP.Modules.Notification.Application.Abstractions.Services;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace DTP.Modules.Notification.Infrastructure.Services
{
    public class EmailNotificationSender : IEmailNotificationSender
    {
        private readonly EmailNotificationSettings _settings;

        public EmailNotificationSender(IOptions<EmailNotificationSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(
            string toEmail,
            string subject,
            string body,
            CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

            message.To.Add(MailboxAddress.Parse(toEmail));

            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = body
            }.ToMessageBody();

            using var client = new SmtpClient();

            var secureSocketOptions = _settings.UseSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.Auto;

            //await client.ConnectAsync(
            //    _settings.Host,
            //    _settings.Port,
            //    secureSocketOptions,
            //    cancellationToken);

            //await client.AuthenticateAsync(
            //    _settings.Username,
            //    _settings.Password,
            //    cancellationToken);

            //await client.SendAsync(message, cancellationToken);

            //await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
