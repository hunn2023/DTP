using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.PhoneCards
{
    public class GetPhoneCardByIdQuery : IRequest<PhoneCardDto?>
    {
        public Guid Id { get; set; }

        public GetPhoneCardByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetPhoneCardByIdQueryHandler
         : IRequestHandler<GetPhoneCardByIdQuery, PhoneCardDto?>
    {
        private readonly IPhoneCardService _phoneCardService;

        public GetPhoneCardByIdQueryHandler(IPhoneCardService phoneCardService)
        {
            _phoneCardService = phoneCardService;
        }

        public async Task<PhoneCardDto?> Handle(
            GetPhoneCardByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _phoneCardService.GetByIdAsync(
                request.Id,
                cancellationToken);
        }
    }
}
