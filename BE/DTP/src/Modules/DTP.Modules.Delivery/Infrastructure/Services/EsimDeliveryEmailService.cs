using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Shared.Application.Emails;
using System.Net;
using System.Text;


namespace DTP.Modules.Delivery.Infrastructure.Services
{
    public class EsimDeliveryEmailService : IEsimDeliveryEmailService
    {
        private readonly IEmailSender _emailSender;

        public EsimDeliveryEmailService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendEsimQrEmailAsync(
            DeliveryDto delivery,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(delivery.CustomerEmail))
                return;

            var subject = $"Thông tin eSIM của bạn - Đơn hàng {delivery.OrderCode}";

            var html = BuildHtml(delivery);

            await _emailSender.SendAsync(
                delivery.CustomerEmail,
                subject,
                html,
                cancellationToken);
        }

        private static string BuildHtml(DeliveryDto delivery)
        {
            var itemsHtml = new StringBuilder();

            foreach (var item in delivery.Items)
            {
                var productName = WebUtility.HtmlEncode(item.ProductName);
                var sku = WebUtility.HtmlEncode(item.Sku ?? "");
                var activationCode = WebUtility.HtmlEncode(item.ActivationCode ?? "");
                var serialNumber = WebUtility.HtmlEncode(item.SerialNumber ?? "");
                var qrCodeUrl = item.QrCodeUrl ?? "";

                itemsHtml.Append($@"
                <div style='border:1px solid #e5e7eb;border-radius:12px;padding:20px;margin-bottom:20px;background:#ffffff;'>
                    <h3 style='margin:0 0 8px 0;color:#111827;font-size:18px;'>{productName}</h3>
                    <p style='margin:0 0 12px 0;color:#6b7280;font-size:14px;'>SKU: {sku}</p>

                    {(string.IsNullOrWhiteSpace(qrCodeUrl) ? "" : $@"
                        <div style='text-align:center;margin:20px 0;'>
                            <img src='{qrCodeUrl}' 
                                 alt='eSIM QR Code' 
                                 style='width:240px;height:240px;max-width:100%;border:1px solid #e5e7eb;border-radius:8px;padding:8px;' />
                        </div>
                    ")}

                    {(string.IsNullOrWhiteSpace(activationCode) ? "" : $@"
                        <p style='font-size:14px;color:#374151;'>
                            <strong>Activation Code:</strong>
                            <span style='font-family:monospace;background:#f3f4f6;padding:4px 8px;border-radius:6px;'>{activationCode}</span>
                        </p>
                    ")}

                    {(string.IsNullOrWhiteSpace(serialNumber) ? "" : $@"
                        <p style='font-size:14px;color:#374151;'>
                            <strong>Serial:</strong>
                            <span style='font-family:monospace;background:#f3f4f6;padding:4px 8px;border-radius:6px;'>{serialNumber}</span>
                        </p>
                    ")}
                </div>
            ");
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8' />
    <title>Thông tin eSIM</title>
</head>
<body style='margin:0;padding:0;background:#f3f4f6;font-family:Arial,Helvetica,sans-serif;'>
    <div style='max-width:680px;margin:0 auto;padding:24px;'>
        <div style='background:#ffffff;border-radius:16px;padding:28px;border:1px solid #e5e7eb;'>
            <h2 style='margin:0 0 12px 0;color:#111827;'>Thanh toán thành công</h2>

            <p style='font-size:15px;color:#374151;line-height:1.6;'>
                Xin chào {WebUtility.HtmlEncode(delivery.CustomerName ?? "Quý khách")},
            </p>

            <p style='font-size:15px;color:#374151;line-height:1.6;'>
                Cảm ơn bạn đã mua hàng. Dưới đây là thông tin eSIM của đơn hàng 
                <strong>{WebUtility.HtmlEncode(delivery.OrderCode)}</strong>.
            </p>

            {itemsHtml}

            <div style='background:#f9fafb;border-radius:12px;padding:16px;margin-top:20px;'>
                <h3 style='margin:0 0 8px 0;color:#111827;font-size:16px;'>Hướng dẫn cài đặt nhanh</h3>
                <ol style='margin:0;padding-left:20px;color:#374151;font-size:14px;line-height:1.7;'>
                    <li>Mở phần Cài đặt trên điện thoại.</li>
                    <li>Chọn Di động / Cellular.</li>
                    <li>Chọn Thêm eSIM / Add eSIM.</li>
                    <li>Quét mã QR trong email này.</li>
                    <li>Làm theo hướng dẫn trên màn hình để kích hoạt.</li>
                </ol>
            </div>

            <p style='font-size:13px;color:#6b7280;line-height:1.6;margin-top:24px;'>
                Lưu ý: Vui lòng không chia sẻ mã QR eSIM cho người khác. Mỗi mã eSIM thường chỉ sử dụng được cho một thiết bị.
            </p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
