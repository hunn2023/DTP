using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;


namespace DTP.Modules.Catalog.Application.Queries.PhoneCards
{
    public class GetPublicPhoneCardBySlugQuery
       : IRequest<PhoneCardDto?>
    {
        public string Slug { get; set; }

        public GetPublicPhoneCardBySlugQuery(string slug)
        {
            Slug = slug;
        }
    }

    public class GetPublicPhoneCardBySlugQueryHandler
        : IRequestHandler<GetPublicPhoneCardBySlugQuery, PhoneCardDto?>
    {
        private readonly IPhoneCardService _phoneCardService;

        public GetPublicPhoneCardBySlugQueryHandler(IPhoneCardService phoneCardService)
        {
            _phoneCardService = phoneCardService;
        }

        public async Task<PhoneCardDto?> Handle(
            GetPublicPhoneCardBySlugQuery request,
            CancellationToken cancellationToken)
        {
            return await _phoneCardService.GetPublicBySlugAsync(
                request.Slug,
                cancellationToken);
        }
    }
}
