using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.PhoneCards
{
    public class UpdatePhoneCardCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public Guid ProviderId { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public decimal FaceValue { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; } = "VND";

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
    public class UpdatePhoneCardCommandHandler
        : IRequestHandler<UpdatePhoneCardCommand, bool>
    {
        private readonly IPhoneCardService _phoneCardService;

        public UpdatePhoneCardCommandHandler(IPhoneCardService phoneCardService)
        {
            _phoneCardService = phoneCardService;
        }

        public async Task<bool> Handle(
            UpdatePhoneCardCommand request,
            CancellationToken cancellationToken)
        {
            return await _phoneCardService.UpdateAsync(
                request,
                cancellationToken);
        }
    }
}
