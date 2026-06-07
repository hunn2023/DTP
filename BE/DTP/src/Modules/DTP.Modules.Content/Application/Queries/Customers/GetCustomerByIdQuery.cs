
using DTP.Modules.Customer.Application.Abstractions.Services;
using DTP.Modules.Customer.Application.DTOs;
using MediatR;

namespace DTP.Modules.Customer.Application.Queries.Customers
{
    public class GetCustomerByIdQuery : IRequest<CustomerDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetCustomerByIdQueryHandler
     : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
    {
        private readonly ICustomerService _customerService;

        public GetCustomerByIdQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<CustomerDto?> Handle(
            GetCustomerByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _customerService.GetByIdAsync(request.Id, cancellationToken);
        }
    }

}
