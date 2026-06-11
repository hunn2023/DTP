using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Category
{
    public class UpdateCategoryCommand : IRequest<Result<CategoryDto>>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Code { get; set; }

        public string Slug { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }
    }

    public class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
    {
        private readonly ICategoryService _categoryService;

        public UpdateCategoryCommandHandler(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<CategoryDto>> Handle(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            return await _categoryService.UpdateAsync(
             request.Id,
             request.Code,
             request.Name,
             request.Slug,
             request.SortOrder,
             cancellationToken);
        }
    }
}
