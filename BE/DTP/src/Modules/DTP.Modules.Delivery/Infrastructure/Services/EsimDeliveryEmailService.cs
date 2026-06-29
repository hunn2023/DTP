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
                var productName = WebUtility.HtmlEncode(item.ProductName ?? "Gói eSIM");
                var sku = WebUtility.HtmlEncode(item.Sku ?? "");
                var activationCode = WebUtility.HtmlEncode(item.ActivationCode ?? "");
                var serialNumber = WebUtility.HtmlEncode(item.SerialNumber ?? "");
                var qrCodeUrl = WebUtility.HtmlEncode(item.QrCodeUrl ?? "");

                itemsHtml.Append($@"
            <div style='
                border:1px solid #e5e7eb;
                border-radius:18px;
                padding:22px;
                margin-bottom:22px;
                background:#ffffff;
                box-shadow:0 8px 24px rgba(15,23,42,0.06);
            '>
                <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse:collapse;'>
                    <tr>
                        <td style='vertical-align:top;'>
                            <div style='
                                display:inline-block;
                                padding:6px 10px;
                                border-radius:999px;
                                background:#ecfeff;
                                color:#0891b2;
                                font-size:12px;
                                font-weight:bold;
                                margin-bottom:10px;
                            '>
                                eSIM READY
                            </div>

                            <h3 style='
                                margin:0 0 6px 0;
                                color:#111827;
                                font-size:20px;
                                line-height:1.35;
                            '>
                                {productName}
                            </h3>

                            {(string.IsNullOrWhiteSpace(sku) ? "" : $@"
                                <p style='margin:0;color:#6b7280;font-size:14px;'>
                                    Mã gói: <strong style='color:#374151;'>{sku}</strong>
                                </p>
                            ")}
                        </td>
                    </tr>
                </table>

                {(string.IsNullOrWhiteSpace(qrCodeUrl) ? "" : $@"
                    <div style='
                        text-align:center;
                        margin:24px 0 18px 0;
                        padding:20px;
                        border-radius:16px;
                        background:#f9fafb;
                        border:1px dashed #d1d5db;
                    '>
                        <p style='
                            margin:0 0 14px 0;
                            color:#374151;
                            font-size:14px;
                            font-weight:bold;
                        '>
                            Quét mã QR để cài đặt eSIM
                        </p>

                        <img src='{qrCodeUrl}'
                             alt='eSIM QR Code'
                             style='
                                width:230px;
                                height:230px;
                                max-width:100%;
                                border:10px solid #ffffff;
                                border-radius:16px;
                                box-shadow:0 6px 18px rgba(15,23,42,0.12);
                             ' />

                        <div style='margin-top:18px;'>
                            <a href='{qrCodeUrl}'
                               style='
                                    display:inline-block;
                                    background:#0f172a;
                                    color:#ffffff;
                                    text-decoration:none;
                                    padding:12px 18px;
                                    border-radius:999px;
                                    font-size:14px;
                                    font-weight:bold;
                               '>
                                Mở mã QR
                            </a>
                        </div>
                    </div>
                ")}

                {(string.IsNullOrWhiteSpace(activationCode) ? "" : $@"
                    <div style='margin-top:16px;'>
                        <p style='
                            margin:0 0 8px 0;
                            font-size:13px;
                            color:#6b7280;
                            font-weight:bold;
                            text-transform:uppercase;
                            letter-spacing:0.04em;
                        '>
                            Activation Code
                        </p>

                        <div style='
                            font-family:Consolas,Monaco,monospace;
                            background:#f3f4f6;
                            border:1px solid #e5e7eb;
                            color:#111827;
                            padding:12px 14px;
                            border-radius:12px;
                            font-size:14px;
                            line-height:1.5;
                            word-break:break-all;
                        '>
                            {activationCode}
                        </div>
                    </div>
                ")}

                {(string.IsNullOrWhiteSpace(serialNumber) ? "" : $@"
                    <div style='margin-top:14px;'>
                        <p style='
                            margin:0 0 8px 0;
                            font-size:13px;
                            color:#6b7280;
                            font-weight:bold;
                            text-transform:uppercase;
                            letter-spacing:0.04em;
                        '>
                            Serial Number
                        </p>

                        <div style='
                            font-family:Consolas,Monaco,monospace;
                            background:#fff7ed;
                            border:1px solid #fed7aa;
                            color:#9a3412;
                            padding:12px 14px;
                            border-radius:12px;
                            font-size:14px;
                            word-break:break-all;
                        '>
                            {serialNumber}
                        </div>
                    </div>
                ")}
            </div>
        ");
            }

            var customerName = WebUtility.HtmlEncode(delivery.CustomerName ?? "Quý khách");
            var orderCode = WebUtility.HtmlEncode(delivery.OrderCode ?? "");

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Thông tin eSIM của bạn</title>
</head>

<body style='margin:0;padding:0;background:#eef2f7;font-family:Arial,Helvetica,sans-serif;'>
    <div style='display:none;font-size:1px;color:#eef2f7;line-height:1px;max-height:0;max-width:0;opacity:0;overflow:hidden;'>
        Thanh toán thành công. eSIM của bạn đã sẵn sàng để cài đặt.
    </div>

    <table width='100%' cellpadding='0' cellspacing='0' style='background:#eef2f7;padding:24px 12px;'>
        <tr>
            <td align='center'>
                <table width='100%' cellpadding='0' cellspacing='0' style='max-width:700px;border-collapse:collapse;'>
                    
                    <tr>
                        <td style='
                            background:linear-gradient(135deg,#0f172a,#0369a1,#0891b2);
                            border-radius:24px 24px 0 0;
                            padding:34px 28px;
                            text-align:center;
                        '>
                            <div style='
                                display:inline-block;
                                padding:8px 14px;
                                border-radius:999px;
                                background:rgba(255,255,255,0.16);
                                color:#ffffff;
                                font-size:13px;
                                font-weight:bold;
                                margin-bottom:14px;
                            '>
                                Thanh toán thành công
                            </div>

                            <h1 style='
                                margin:0;
                                color:#ffffff;
                                font-size:28px;
                                line-height:1.3;
                            '>
                                eSIM của bạn đã sẵn sàng
                            </h1>

                            <p style='
                                margin:12px 0 0 0;
                                color:#dbeafe;
                                font-size:15px;
                                line-height:1.6;
                            '>
                                Cảm ơn bạn đã mua hàng tại EZSIM
                            </p>
                        </td>
                    </tr>

                    <tr>
                        <td style='
                            background:#ffffff;
                            padding:28px;
                            border-left:1px solid #e5e7eb;
                            border-right:1px solid #e5e7eb;
                        '>
                            <p style='font-size:15px;color:#374151;line-height:1.7;margin:0 0 14px 0;'>
                                Xin chào <strong style='color:#111827;'>{customerName}</strong>,
                            </p>

                            <p style='font-size:15px;color:#374151;line-height:1.7;margin:0 0 20px 0;'>
                                Đơn hàng <strong style='color:#111827;'>{orderCode}</strong> đã được xử lý thành công.
                                Dưới đây là thông tin eSIM của bạn. Vui lòng lưu email này để sử dụng khi cần cài đặt lại hoặc kiểm tra thông tin.
                            </p>

                            <div style='
                                background:#f0fdf4;
                                border:1px solid #bbf7d0;
                                border-radius:14px;
                                padding:14px 16px;
                                margin-bottom:24px;
                            '>
                                <p style='margin:0;color:#166534;font-size:14px;line-height:1.6;'>
                                    <strong>Lưu ý quan trọng:</strong> Hãy cài đặt eSIM khi điện thoại có kết nối Wi-Fi ổn định.
                                </p>
                            </div>

                            {itemsHtml}

                            <div style='
                                background:#f9fafb;
                                border:1px solid #e5e7eb;
                                border-radius:18px;
                                padding:22px;
                                margin-top:24px;
                            '>
                                <h3 style='
                                    margin:0 0 16px 0;
                                    color:#111827;
                                    font-size:18px;
                                '>
                                    Hướng dẫn cài đặt nhanh
                                </h3>

                                <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse:collapse;'>
                                    <tr>
                                        <td style='padding:0 0 12px 0;'>
                                            <div style='font-size:14px;color:#374151;line-height:1.7;'>
                                                <strong>iPhone:</strong> Cài đặt → Di động → Thêm eSIM → Sử dụng mã QR.
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding:0 0 12px 0;'>
                                            <div style='font-size:14px;color:#374151;line-height:1.7;'>
                                                <strong>Android:</strong> Cài đặt → Kết nối → Quản lý SIM → Thêm eSIM.
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div style='font-size:14px;color:#374151;line-height:1.7;'>
                                                Sau khi cài đặt, bật chuyển vùng dữ liệu nếu gói eSIM yêu cầu sử dụng tại nước ngoài.
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <div style='
                                background:#fff7ed;
                                border:1px solid #fed7aa;
                                border-radius:16px;
                                padding:16px;
                                margin-top:22px;
                            '>
                                <p style='margin:0;color:#9a3412;font-size:13px;line-height:1.7;'>
                                    <strong>Bảo mật:</strong> Không chia sẻ mã QR, Activation Code hoặc Serial Number cho người khác.
                                    Mỗi mã eSIM thường chỉ cài đặt được trên một thiết bị.
                                </p>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td style='
                            background:#ffffff;
                            border-radius:0 0 24px 24px;
                            padding:22px 28px;
                            text-align:center;
                            border:1px solid #e5e7eb;
                            border-top:none;
                        '>
                            <p style='margin:0 0 8px 0;color:#6b7280;font-size:13px;line-height:1.6;'>
                                Cần hỗ trợ? Vui lòng liên hệ bộ phận chăm sóc khách hàng của EZSIM.
                            </p>

                            <p style='margin:0;color:#9ca3af;font-size:12px;line-height:1.6;'>
                                Email này được gửi tự động sau khi đơn hàng thanh toán thành công.
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
