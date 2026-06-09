using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.ProductVariantFeatures
{
    public class CreateProductVariantFeatureCommand : IRequest<Result<Guid>>
    {
        public Guid ProductVariantId { get; set; }

        public string Text { get; set; } = default!;

        public string? Icon { get; set; }

        public int? SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CreateProductVariantFeatureCommandHandler
       : IRequestHandler<CreateProductVariantFeatureCommand, Result<Guid>>
    {
        private readonly IProductVariantFeatureService _service;

        public CreateProductVariantFeatureCommandHandler(
            IProductVariantFeatureService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(
            CreateProductVariantFeatureCommand request,
            CancellationToken cancellationToken)
        {
            return await _service.CreateAsync(
                request.ProductVariantId,
                request.Text,
                request.Icon,
                request.SortOrder,
                request.IsActive,
                cancellationToken);
        }
    }
}
