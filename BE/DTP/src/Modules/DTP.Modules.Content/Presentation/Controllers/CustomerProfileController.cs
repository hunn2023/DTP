using DTP.Modules.Content.Application.Commands.CustomerAddresses;
using DTP.Modules.Content.Application.Queries.CustomerAddresses;
using DTP.Modules.Content.Application.Queries.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Presentation.Controllers
{
    [ApiController]
    [Route("api/customer/profile")]
    [Authorize]
    public class CustomerProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyProfile(
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var result = await _mediator.Send(new GetMyCustomerProfileQuery
            {
                UserId = userId
            }, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetMyAddresses(
            CancellationToken cancellationToken = default)
        {
            var customerId = await GetCurrentCustomerIdAsync(cancellationToken);

            var result = await _mediator.Send(new GetCustomerAddressesQuery
            {
                CustomerId = customerId
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("addresses/default")]
        public async Task<IActionResult> GetDefaultAddress(
            CancellationToken cancellationToken = default)
        {
            var customerId = await GetCurrentCustomerIdAsync(cancellationToken);

            var result = await _mediator.Send(new GetDefaultCustomerAddressQuery
            {
                CustomerId = customerId
            }, cancellationToken);

            return Ok(result);
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> CreateAddress(
            [FromBody] CreateCustomerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            var customerId = await GetCurrentCustomerIdAsync(cancellationToken);
            command.CustomerId = customerId;

            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }

        [HttpPut("addresses/{id:guid}")]
        public async Task<IActionResult> UpdateAddress(
            Guid id,
            [FromBody] UpdateCustomerAddressCommand command,
            CancellationToken cancellationToken = default)
        {
            command.Id = id;

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("addresses/{id:guid}/default")]
        public async Task<IActionResult> SetDefaultAddress(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var customerId = await GetCurrentCustomerIdAsync(cancellationToken);

            var result = await _mediator.Send(new SetDefaultCustomerAddressCommand
            {
                CustomerId = customerId,
                AddressId = id
            }, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("addresses/{id:guid}")]
        public async Task<IActionResult> DeleteAddress(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new DeleteCustomerAddressCommand
            {
                Id = id
            }, cancellationToken);

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? User.FindFirstValue("userId");

            if (string.IsNullOrWhiteSpace(userIdValue))
                throw new UnauthorizedAccessException("UserId not found in token.");

            return Guid.Parse(userIdValue);
        }

        private async Task<Guid> GetCurrentCustomerIdAsync(
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            var customer = await _mediator.Send(new GetMyCustomerProfileQuery
            {
                UserId = userId
            }, cancellationToken);

            if (customer == null)
                throw new Exception("Customer profile not found.");

            return customer.Id;
        }
    }
}
