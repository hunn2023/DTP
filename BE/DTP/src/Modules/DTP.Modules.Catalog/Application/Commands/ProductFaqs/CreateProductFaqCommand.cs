using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.ProductFaqs
{
    public class CreateProductFaqCommand : IRequest<Result<ProductFaqDto>>
    {
        public Guid ProductId { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CreateProductFaqCommandHandler
       : IRequestHandler<CreateProductFaqCommand, Result<ProductFaqDto>>
    {
        private readonly IProductFaqService _productFaqService;

        public CreateProductFaqCommandHandler(
            IProductFaqService productFaqService)
        {
            _productFaqService = productFaqService;
        }

        public async Task<Result<ProductFaqDto>> Handle(
            CreateProductFaqCommand request,
            CancellationToken cancellationToken)
        {
            var dto = new CreateProductFaqDto
            {
                ProductId = request.ProductId,
                Question = request.Question,
                Answer = request.Answer,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive
            };

            return await _productFaqService.CreateAsync(
                dto,
                cancellationToken);
        }
    }
}
