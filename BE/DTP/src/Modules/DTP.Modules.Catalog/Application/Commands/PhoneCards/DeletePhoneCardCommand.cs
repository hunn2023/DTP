using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.PhoneCards
{
    public class DeletePhoneCardCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeletePhoneCardCommand(Guid id)
        {
            Id = id;
        }
    }
    public class DeletePhoneCardCommandHandler
      : IRequestHandler<DeletePhoneCardCommand, bool>
    {
        private readonly IPhoneCardService _phoneCardService;

        public DeletePhoneCardCommandHandler(IPhoneCardService phoneCardService)
        {
            _phoneCardService = phoneCardService;
        }

        public async Task<bool> Handle(
            DeletePhoneCardCommand request,
            CancellationToken cancellationToken)
        {
            return await _phoneCardService.DeleteAsync(
                request.Id,
                cancellationToken);
        }
    }
}
