using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.ProductContents
{
    public class DeleteProductContentCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public DeleteProductContentCommand(Guid id)
        {
            Id = id;
        }
    }


    public class DeleteProductContentCommandHandler
        : IRequestHandler<DeleteProductContentCommand, Result>
    {
        private readonly IProductContentService _productContentService;

        public DeleteProductContentCommandHandler(
            IProductContentService productContentService)
        {
            _productContentService = productContentService;
        }

        public async Task<Result> Handle(
            DeleteProductContentCommand request,
            CancellationToken cancellationToken)
        {
            return await _productContentService.DeleteAsync(
                 request.Id,
                 cancellationToken);

        }
    }
}
