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

            var customerName = System.Net.WebUtility.HtmlEncode(order.CustomerName ?? "Quý khách");
            var orderCode = System.Net.WebUtility.HtmlEncode(order.OrderCode);
            var packageName = System.Net.WebUtility.HtmlEncode(redeem.PackageName ?? redeem.Sku ?? "Gói eSIM");
            var iccid = System.Net.WebUtility.HtmlEncode(redeem.Iccid ?? "-");
            var apn = System.Net.WebUtility.HtmlEncode(redeem.Apn ?? "-");
            var activationCode = System.Net.WebUtility.HtmlEncode(redeem.ActivationCode ?? "-");

            var qrCodeUrl = System.Net.WebUtility.HtmlEncode(redeem.QrCodeUrl ?? "#");
            var shortUrlApple = System.Net.WebUtility.HtmlEncode(redeem.ShortUrlApple ?? "#");
            var shortUrlAndroid = System.Net.WebUtility.HtmlEncode(redeem.ShortUrlAndroid ?? "#");

            var body = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Thông tin eSIM của bạn</title>
</head>

<body style=""margin:0;padding:0;background-color:#eef7f5;font-family:Arial,Helvetica,sans-serif;color:#111827;"">

    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation""
           style=""width:100%;background-color:#eef7f5;padding:36px 16px;"">
        <tr>
            <td align=""center"">

                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation""
                       style=""max-width:600px;background-color:#ffffff;border-radius:18px;overflow:hidden;box-shadow:0 12px 32px rgba(15,23,42,0.10);"">

                    <!-- Header -->
                    <tr>
                        <td style=""background:linear-gradient(135deg,#00b894,#00cec9);padding:34px 32px;text-align:center;"">
                            <div style=""font-size:30px;line-height:36px;font-weight:800;color:#ffffff;letter-spacing:-0.6px;"">
                                ezsim
                            </div>
                            <div style=""margin-top:8px;font-size:14px;line-height:20px;color:#eafffb;"">
                                eSIM du lịch của bạn đã sẵn sàng
                            </div>
                        </td>
                    </tr>

                    <!-- Title -->
                    <tr>
                        <td style=""padding:34px 32px 0 32px;text-align:center;"">
                            <h1 style=""margin:0;font-size:26px;line-height:34px;font-weight:800;color:#111827;"">
                                Thông tin eSIM của bạn
                            </h1>
                        </td>
                    </tr>

                    <!-- Greeting -->
                    <tr>
                        <td style=""padding:16px 36px 0 36px;text-align:center;"">
                            <p style=""margin:0;font-size:16px;line-height:25px;color:#4b5563;"">
                                Xin chào <strong style=""color:#111827;"">{customerName}</strong>,<br />
                                Đơn hàng <strong style=""color:#047857;"">{orderCode}</strong> đã được xử lý thành công.
                            </p>
                        </td>
                    </tr>

                    <!-- Package Info -->
                    <tr>
                        <td style=""padding:30px 32px 0 32px;"">
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation""
                                   style=""background-color:#f9fafb;border:1px solid #e5e7eb;border-radius:14px;"">
                                <tr>
                                    <td style=""padding:20px;"">
                                        <h2 style=""margin:0 0 16px 0;font-size:18px;line-height:24px;color:#111827;"">
                                            Chi tiết gói eSIM
                                        </h2>

                                        <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                                            <tr>
                                                <td style=""padding:8px 0;font-size:14px;color:#6b7280;width:110px;"">Gói</td>
                                                <td style=""padding:8px 0;font-size:14px;color:#111827;font-weight:700;"">{packageName}</td>
                                            </tr>
                                            <tr>
                                                <td style=""padding:8px 0;font-size:14px;color:#6b7280;"">ICCID</td>
                                                <td style=""padding:8px 0;font-size:14px;color:#111827;font-weight:700;"">{iccid}</td>
                                            </tr>
                                            <tr>
                                                <td style=""padding:8px 0;font-size:14px;color:#6b7280;"">APN</td>
                                                <td style=""padding:8px 0;font-size:14px;color:#111827;font-weight:700;"">{apn}</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- QR Code -->
                    <tr>
                        <td style=""padding:30px 32px 0 32px;text-align:center;"">
                            <h2 style=""margin:0 0 14px 0;font-size:18px;line-height:24px;color:#111827;"">
                                QR Code eSIM
                            </h2>

                            <p style=""margin:0 0 18px 0;font-size:14px;line-height:22px;color:#4b5563;"">
                                Quét mã QR bên dưới để cài đặt eSIM trên thiết bị của bạn.
                            </p>

                            <div style=""display:inline-block;padding:14px;background-color:#ffffff;border:1px solid #e5e7eb;border-radius:16px;"">
                                <img src=""{qrCodeUrl}""
                                     alt=""QR Code eSIM""
                                     width=""220""
                                     style=""display:block;width:220px;max-width:100%;height:auto;border:0;"" />
                            </div>

                            <div style=""margin-top:20px;"">
                                <a href=""{qrCodeUrl}""
                                   style=""display:inline-block;background-color:#00b894;color:#ffffff;text-decoration:none;font-size:15px;font-weight:700;padding:14px 24px;border-radius:999px;"">
                                    Mở QR Code eSIM
                                </a>
                            </div>
                        </td>
                    </tr>

                    <!-- Activation Code -->
                    <tr>
                        <td style=""padding:30px 32px 0 32px;"">
                            <h2 style=""margin:0 0 12px 0;font-size:18px;line-height:24px;color:#111827;text-align:center;"">
                                Activation Code
                            </h2>

                            <div style=""background-color:#ecfdf5;border:1px solid #a7f3d0;border-radius:14px;padding:18px 20px;text-align:center;"">
                                <p style=""margin:0;font-size:14px;line-height:22px;color:#047857;font-weight:700;word-break:break-all;"">
                                    {activationCode}
                                </p>
                            </div>
                        </td>
                    </tr>

                    <!-- Quick Install -->
                    <tr>
                        <td style=""padding:30px 32px 0 32px;"">
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"">
                                <tr>
                                    <td align=""center"" style=""padding:0 0 12px 0;"">
                                        <h2 style=""margin:0;font-size:18px;line-height:24px;color:#111827;"">
                                            Cài đặt nhanh
                                        </h2>
                                    </td>
                                </tr>
                                <tr>
                                    <td align=""center"" style=""padding:0;"">
                                        <a href=""{shortUrlApple}""
                                           style=""display:inline-block;margin:6px;background-color:#111827;color:#ffffff;text-decoration:none;font-size:14px;font-weight:700;padding:12px 18px;border-radius:999px;"">
                                            Cài nhanh iOS
                                        </a>

                                        <a href=""{shortUrlAndroid}""
                                           style=""display:inline-block;margin:6px;background-color:#047857;color:#ffffff;text-decoration:none;font-size:14px;font-weight:700;padding:12px 18px;border-radius:999px;"">
                                            Cài nhanh Android
                                        </a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Note -->
                    <tr>
                        <td style=""padding:30px 32px 34px 32px;"">
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation""
                                   style=""background-color:#fffbeb;border:1px solid #fde68a;border-radius:14px;"">
                                <tr>
                                    <td style=""padding:18px 20px;"">
                                        <p style=""margin:0;font-size:14px;line-height:22px;color:#92400e;"">
                                            <strong>Lưu ý:</strong>
                                            Vui lòng không xoá email này cho đến khi bạn đã cài đặt và kích hoạt eSIM thành công.
                                            Nếu không thấy hình ảnh QR, hãy bấm nút <strong>Mở QR Code eSIM</strong>.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style=""background-color:#f3f4f6;padding:22px 32px;text-align:center;"">
                            <p style=""margin:0;font-size:12px;line-height:18px;color:#6b7280;"">
                                Email này được gửi tự động từ EZSIM. Vui lòng không trả lời email này.
                            </p>
                            <p style=""margin:8px 0 0 0;font-size:12px;line-height:18px;color:#9ca3af;"">
                                © EZSIM
                            </p>
                        </td>
                    </tr>

                </table>

            </td>
        </tr>
    </table>

</body>
</html>";

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