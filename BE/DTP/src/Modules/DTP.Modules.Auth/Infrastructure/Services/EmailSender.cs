using DTP.Modules.Auth.Application.Abstractions.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Infrastructure.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; }
        public bool UseSsl { get; set; }

        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;

        public string FromEmail { get; set; } = default!;
        public string FromName { get; set; } = "DTP";
    }


    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendOtpAsync(
            string toEmail,
            string otp,
            CancellationToken cancellationToken = default)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

            message.To.Add(MailboxAddress.Parse(toEmail));

            message.Subject = "Mã xác thực đăng ký DTP";

            message.Body = new BodyBuilder
            {
                HtmlBody = $@"
                <div style='font-family:Arial,sans-serif'>
                    <h2>Xác thực tài khoản DTP</h2>
                    <p>Mã OTP của bạn là:</p>
                    <h1 style='letter-spacing:4px'>{otp}</h1>
                    <p>Mã này có hiệu lực trong 10 phút.</p>
                    <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>
                </div>"
            }.ToMessageBody();

            using var client = new SmtpClient();

            var secureSocketOptions = _settings.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                secureSocketOptions,
                cancellationToken);

            await client.AuthenticateAsync(
                _settings.UserName,
                _settings.Password,
                cancellationToken);

            await client.SendAsync(message, cancellationToken);

            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
