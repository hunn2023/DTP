using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Carriers
{
    public class DeleteCarrierCommand : IRequest<Result>
    {
        public Guid Id { get; }

        public DeleteCarrierCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteCarrierCommandHandler
    : IRequestHandler<DeleteCarrierCommand, Result>
    {
        private readonly ICarrierService _carrierService;

        public DeleteCarrierCommandHandler(ICarrierService carrierService)
        {
            _carrierService = carrierService;
        }

        public async Task<Result> Handle(
            DeleteCarrierCommand request,
            CancellationToken cancellationToken)
        {
            return await _carrierService.DeleteAsync(
                 request.Id,
                 cancellationToken);

        }
    }
}
