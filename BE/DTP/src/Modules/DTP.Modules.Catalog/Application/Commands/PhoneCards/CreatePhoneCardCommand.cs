using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.PhoneCards
{
    public class CreatePhoneCardCommand : IRequest<Guid>
    {
        public Guid ProductVariantId { get; set; }

        public Guid ProviderId { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public decimal FaceValue { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; } = "VND";

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CreatePhoneCardCommandHandler
       : IRequestHandler<CreatePhoneCardCommand, Guid>
    {
        private readonly IPhoneCardService _phoneCardService;

        public CreatePhoneCardCommandHandler(IPhoneCardService phoneCardService)
        {
            _phoneCardService = phoneCardService;
        }

        public async Task<Guid> Handle(
            CreatePhoneCardCommand request,
            CancellationToken cancellationToken)
        {
            return await _phoneCardService.CreateAsync(
                request,
                cancellationToken);
        }
    }
}
