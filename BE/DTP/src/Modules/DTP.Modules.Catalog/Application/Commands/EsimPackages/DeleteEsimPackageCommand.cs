using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.EsimPackages
{
    public class DeleteEsimPackageCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public DeleteEsimPackageCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteEsimPackageCommandHandler
       : IRequestHandler<DeleteEsimPackageCommand, Result>
    {
        private readonly IEsimPackageService _esimPackageService;

        public DeleteEsimPackageCommandHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<Result> Handle(
            DeleteEsimPackageCommand request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.DeleteAsync(
                request.Id,
                cancellationToken);
        }
    }
}
