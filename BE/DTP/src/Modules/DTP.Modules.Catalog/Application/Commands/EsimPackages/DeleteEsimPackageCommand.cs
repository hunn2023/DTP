using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.EsimPackages
{
    public class DeleteEsimPackageCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteEsimPackageCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteEsimPackageCommandHandler
       : IRequestHandler<DeleteEsimPackageCommand, bool>
    {
        private readonly IEsimPackageService _esimPackageService;

        public DeleteEsimPackageCommandHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<bool> Handle(
            DeleteEsimPackageCommand request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.DeleteAsync(
                request.Id,
                cancellationToken);
        }
    }
}
