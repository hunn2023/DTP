using Amazon.Runtime.Internal;
using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Presentation.Controllers
{
    [ApiController]
    [Route("api/provider-webhooks")]
    public class ProviderWebhookController : ControllerBase
    {
        private readonly IExternalProviderRepository _providerRepository;
        private readonly IProviderWebhookLogRepository _webhookLogRepository;
        private readonly IProviderUnitOfWork _unitOfWork;

        public ProviderWebhookController(
            IExternalProviderRepository providerRepository,
            IProviderWebhookLogRepository webhookLogRepository,
            IProviderUnitOfWork unitOfWork)
        {
            _providerRepository = providerRepository;
            _webhookLogRepository = webhookLogRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("{providerCode}")]
        public async Task<IActionResult> Receive(
            string providerCode,
            CancellationToken cancellationToken)
        {
            providerCode = providerCode.Trim().ToUpperInvariant();

            var provider = await _providerRepository.GetByCodeAsync(
                providerCode,
                cancellationToken);

            if (provider == null)
                return NotFound("Provider not found.");

            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync(cancellationToken);

            var eventType = Request.Headers.TryGetValue("X-Event-Type", out var eventTypeHeader)
                ? eventTypeHeader.ToString()
                : "unknown";

            var log = new ProviderWebhookLog(
                provider.Id,
                eventType,
                payload);

            await _webhookLogRepository.AddAsync(log, cancellationToken);

            try
            {
                // TODO:
                // 1. Verify signature nếu provider có
                // 2. Parse payload
                // 3. Update ProviderOrder / ProviderOrderItem
                // 4. Trigger Delivery / Notification

                log.MarkProcessed();
            }
            catch (Exception ex)
            {
                log.MarkFailed(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Ok(new
            {
                received = true
            });
        }
    }
}
