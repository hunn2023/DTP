using DTP.Modules.Content.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Commands.Seo
{
    public record DeleteSeoMetadataCommand(Guid Id) : IRequest<bool>;

    public class DeleteSeoMetadataCommandHandler
    : IRequestHandler<DeleteSeoMetadataCommand, bool>
    {
        private readonly ISeoMetadataService _service;

        public DeleteSeoMetadataCommandHandler(ISeoMetadataService service)
        {
            _service = service;
        }

        public Task<bool> Handle(
            DeleteSeoMetadataCommand request,
            CancellationToken cancellationToken)
        {
            return _service.DeleteAsync(
                request.Id,
                cancellationToken);
        }
    }
}
