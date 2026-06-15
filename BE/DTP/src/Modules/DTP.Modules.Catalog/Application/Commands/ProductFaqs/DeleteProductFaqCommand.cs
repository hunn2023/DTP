using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.ProductFaqs
{

    public class DeleteProductFaqCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public DeleteProductFaqCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteProductFaqCommandHandler
      : IRequestHandler<DeleteProductFaqCommand, Result>
    {
        private readonly IProductFaqService _productFaqService;

        public DeleteProductFaqCommandHandler(
            IProductFaqService productFaqService)
        {
            _productFaqService = productFaqService;
        }

        public async Task<Result> Handle(
            DeleteProductFaqCommand request,
            CancellationToken cancellationToken)
        {
            return await _productFaqService.DeleteAsync(
                 request.Id,
                 cancellationToken);

        }
    }
}
