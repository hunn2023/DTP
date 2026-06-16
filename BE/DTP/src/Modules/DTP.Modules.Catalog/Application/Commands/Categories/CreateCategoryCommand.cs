using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Category
{
    public class CreateCategoryCommand : IRequest<Result<CategoryDto>>
    {
        public string Name { get; set; } = string.Empty;

        public string? Code { get; set; }

        public string? Slug { get; set; }

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; }

    }

    public class CreateCategoryCommandHandler
   : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryCommandHandler(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<CategoryDto>> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            return await _categoryService.CreateAsync(
             request.Code,
             request.Name,
             request.Slug,
             request.SortOrder,
             cancellationToken);
        }
    }
}
