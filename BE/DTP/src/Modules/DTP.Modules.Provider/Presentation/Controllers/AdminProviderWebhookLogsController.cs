using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/provider-webhook-logs")]
    public class AdminProviderWebhookLogsController : ControllerBase
    {
        private readonly IProviderWebhookLogRepository _repository;

        public AdminProviderWebhookLogsController(IProviderWebhookLogRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] Guid? providerId,
            [FromQuery] ProviderWebhookStatus? status,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetPagedAsync(
                providerId,
                status,
                fromDate,
                toDate,
                pageIndex,
                pageSize,
                cancellationToken);

            return Ok(result);
        }
    }
}
