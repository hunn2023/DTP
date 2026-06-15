using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Shared.Application.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.Services
{
    public class ProviderDeliveryEmailService : IProviderDeliveryEmailService
    {
        private readonly IProviderOrderReader _orderReader;
        private readonly IEmailSender _emailSender;

        public ProviderDeliveryEmailService(
            IProviderOrderReader orderReader,
            IEmailSender emailSender)
        {
            _orderReader = orderReader;
            _emailSender = emailSender;
        }

        public async Task SendEsimEmailAsync(
            ProviderRedeem redeem,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderReader.GetOrderForReservationAsync(
                redeem.DtpOrderId,
                cancellationToken);

            if (order is null)
                throw new InvalidOperationException("Không tìm thấy order để gửi email eSIM.");

            var body = $@"
<h2>Thông tin eSIM của bạn</h2>
<p>Xin chào {order.CustomerName},</p>
<p>Đơn hàng <strong>{order.OrderCode}</strong> đã được xử lý thành công.</p>

<p><strong>Gói:</strong> {redeem.PackageName ?? redeem.Sku}</p>
<p><strong>ICCID:</strong> {redeem.Iccid}</p>
<p><strong>APN:</strong> {redeem.Apn}</p>

<p><strong>QR Code:</strong></p>
<p><a href=""{redeem.QrCodeUrl}"">Mở QR Code eSIM</a></p>

<p><strong>Activation Code:</strong></p>
<p>{redeem.ActivationCode}</p>

<p><strong>Cài nhanh iOS:</strong> <a href=""{redeem.ShortUrlApple}"">{redeem.ShortUrlApple}</a></p>
<p><strong>Cài nhanh Android:</strong> <a href=""{redeem.ShortUrlAndroid}"">{redeem.ShortUrlAndroid}</a></p>
";

            await _emailSender.SendAsync(
                order.CustomerEmail,
                $"Thông tin eSIM đơn hàng {order.OrderCode}",
                body,
                cancellationToken);
        }

        public async Task SendInsuranceEmailAsync(
            ProviderRedeem redeem,
            CancellationToken cancellationToken = default)
        {
            var order = await _orderReader.GetOrderForReservationAsync(
                redeem.DtpOrderId,
                cancellationToken);

            if (order is null)
                throw new InvalidOperationException("Không tìm thấy order để gửi email bảo hiểm.");

            var body = $@"
<h2>Thông tin bảo hiểm của bạn</h2>
<p>Xin chào {order.CustomerName},</p>
<p>Đơn hàng <strong>{order.OrderCode}</strong> đã được xử lý thành công.</p>

<p><strong>Gói:</strong> {redeem.PackageName ?? redeem.Sku}</p>
<p><strong>Số hợp đồng:</strong> {redeem.PolicyNumber}</p>
<p><strong>Link chứng nhận:</strong> <a href=""{redeem.PolicyUrl}"">{redeem.PolicyUrl}</a></p>
<p><strong>Policy GCN:</strong> {redeem.PolicyCertificate}</p>
";

            await _emailSender.SendAsync(
                order.CustomerEmail,
                $"Thông tin bảo hiểm đơn hàng {order.OrderCode}",
                body,
                cancellationToken);
        }
    }
}