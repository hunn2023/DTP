using DTP.Shared.Application.Emails;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Infrastructure.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendAsync(
            string to,
            string subject,
            string htmlBody,
            CancellationToken cancellationToken = default)
        {
            var message = new EmailMessage
            {
                To = to,
                Subject = subject,
                HtmlBody = htmlBody
            };

            return SendAsync(message, cancellationToken);
        }

        public async Task SendAsync(
            EmailMessage message,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(message.To))
                throw new InvalidOperationException("Email người nhận không hợp lệ.");

            if (string.IsNullOrWhiteSpace(message.Subject))
                throw new InvalidOperationException("Subject email không hợp lệ.");

            if (string.IsNullOrWhiteSpace(message.HtmlBody))
                throw new InvalidOperationException("Nội dung email không hợp lệ.");

            var host = _configuration["Email:Host"];
            var port = int.Parse(_configuration["Email:Port"] ?? "587");
            var useSsl = bool.Parse(_configuration["Email:UseSsl"] ?? "false");
            var userName = _configuration["Email:UserName"];
            var password = _configuration["Email:Password"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"] ?? "DTP eSIM";

            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("Email:Smtp:Host chưa được cấu hình.");

            if (string.IsNullOrWhiteSpace(userName))
                throw new InvalidOperationException("Email:Smtp:UserName chưa được cấu hình.");

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Email:Smtp:Password chưa được cấu hình.");

            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new InvalidOperationException("Email:Smtp:FromEmail chưa được cấu hình.");

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(userName, password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = message.Subject,
                Body = message.HtmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(message.To);

            await client.SendMailAsync(mail, cancellationToken);
        }
    }
}
