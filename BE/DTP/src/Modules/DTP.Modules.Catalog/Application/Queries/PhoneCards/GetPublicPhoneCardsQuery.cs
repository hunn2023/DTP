using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.PhoneCards
{
    public class GetPublicPhoneCardsQuery
         : IRequest<PagedResultDto<PhoneCardDto>>
    {
        public Guid? ProviderId { get; set; }

        public decimal? MinFaceValue { get; set; }

        public decimal? MaxFaceValue { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetPublicPhoneCardsQueryHandler
       : IRequestHandler<GetPublicPhoneCardsQuery, PagedResultDto<PhoneCardDto>>
    {
        private readonly IPhoneCardService _phoneCardService;

        public GetPublicPhoneCardsQueryHandler(IPhoneCardService phoneCardService)
        {
            _phoneCardService = phoneCardService;
        }

        public async Task<PagedResultDto<PhoneCardDto>> Handle(
            GetPublicPhoneCardsQuery request,
            CancellationToken cancellationToken)
        {
            return await _phoneCardService.GetPublicPagedAsync(
                request.ProviderId,
                request.MinFaceValue,
                request.MaxFaceValue,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
