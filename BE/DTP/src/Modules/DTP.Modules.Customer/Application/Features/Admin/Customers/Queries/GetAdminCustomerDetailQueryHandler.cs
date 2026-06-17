using DTP.Modules.Customer.Application.Abstractions.Repositories;
using DTP.Modules.Customer.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Customer.Application.Features.Admin.Customers.Queries
{
    public class GetAdminCustomerDetailQueryHandler
        : IRequestHandler<GetAdminCustomerDetailQuery, Result<AdminCustomerDetailDto>>
    {
        private readonly IAdminCustomerRepository _repository;

        public GetAdminCustomerDetailQueryHandler(IAdminCustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<AdminCustomerDetailDto>> Handle(
            GetAdminCustomerDetailQuery request,
            CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
                return Result<AdminCustomerDetailDto>.Failure("UserId không hợp lệ.");

            var customer = await _repository.GetDetailAsync(
                request.UserId,
                cancellationToken);

            if (customer == null)
                return Result<AdminCustomerDetailDto>.Failure("Không tìm thấy khách hàng.");

            return Result<AdminCustomerDetailDto>.Success(customer);
        }
    }
}
