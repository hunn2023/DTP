using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.PhoneCards
{
    public class GetAdminPhoneCardsQuery
       : IRequest<PagedResultDto<PhoneCardDto>>
    {
        public string? Keyword { get; set; }

        public Guid? ProductVariantId { get; set; }

        public Guid? ProviderId { get; set; }

        public bool? IsActive { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetAdminPhoneCardsQueryHandler
       : IRequestHandler<GetAdminPhoneCardsQuery, PagedResultDto<PhoneCardDto>>
    {
        private readonly IPhoneCardService _phoneCardService;

        public GetAdminPhoneCardsQueryHandler(IPhoneCardService phoneCardService)
        {
            _phoneCardService = phoneCardService;
        }

        public async Task<PagedResultDto<PhoneCardDto>> Handle(
            GetAdminPhoneCardsQuery request,
            CancellationToken cancellationToken)
        {
            return await _phoneCardService.GetPagedAsync(
                request.Keyword,
                request.ProductVariantId,
                request.ProviderId,
                request.IsActive,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
