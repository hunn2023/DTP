using DTP.Modules.Payment.Application.Commands.PaymentProviders;
using DTP.Modules.Payment.Application.Queries.PaymentProviders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/payment-providers")]
    [Authorize(Roles = "ADMIN")]
    public class AdminPaymentProvidersController : ControllerBase
    {
        private readonly ISender _sender;

        public AdminPaymentProvidersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentProviders(
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetAdminPaymentProvidersQuery(),
                cancellationToken);

            return Ok(new
            {
                isSuccess = true,
                data = result
            });
        }

        [HttpPatch("{id:guid}/active")]
        public async Task<IActionResult> SetActive(
            Guid id,
            [FromBody] SetPaymentProviderActiveRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new SetPaymentProviderActiveCommand(id, request.IsActive),
                cancellationToken);

            return Ok(new
            {
                isSuccess = true,
                message = request.IsActive
                    ? "Đã bật phương thức thanh toán."
                    : "Đã tắt phương thức thanh toán."
            });
        }

        [HttpPatch("{id:guid}/default")]
        public async Task<IActionResult> SetDefault(
            Guid id,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new SetDefaultPaymentProviderCommand(id),
                cancellationToken);

            return Ok(new
            {
                isSuccess = true,
                message = "Đã đặt phương thức thanh toán mặc định."
            });
        }

        [HttpPatch("{id:guid}/limits")]
        public async Task<IActionResult> UpdateLimits(
            Guid id,
            [FromBody] UpdatePaymentProviderLimitsRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new UpdatePaymentProviderLimitsCommand(
                    id,
                    request.MinAmount,
                    request.MaxAmount),
                cancellationToken);

            return Ok(new
            {
                isSuccess = true,
                message = "Đã cập nhật giới hạn tiền."
            });
        }

        [HttpPatch("{id:guid}/sort-order")]
        public async Task<IActionResult> UpdateSortOrder(
            Guid id,
            [FromBody] UpdatePaymentProviderSortOrderRequest request,
            CancellationToken cancellationToken)
        {
            await _sender.Send(
                new UpdatePaymentProviderSortOrderCommand(
                    id,
                    request.SortOrder),
                cancellationToken);

            return Ok(new
            {
                isSuccess = true,
                message = "Đã cập nhật thứ tự hiển thị."
            });
        }
    }

    public class SetPaymentProviderActiveRequest
    {
        public bool IsActive { get; set; }
    }

    public class UpdatePaymentProviderLimitsRequest
    {
        public decimal? MinAmount { get; set; }

        public decimal? MaxAmount { get; set; }
    }

    public class UpdatePaymentProviderSortOrderRequest
    {
        public int SortOrder { get; set; }
    }
}
