using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Category
{
    public class CreateCategoryCommand : IRequest<CategoryDto>
    {
        public string Name { get; set; } = string.Empty;

        public string? Code { get; set; }

        public string Slug { get; set; }
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int SortOrder { get; set; }

    }

    public class CreateCategoryCommandHandler
   : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryService _categoryService;

        public CreateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ICategoryService categoryService)
        {
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
        }

        public async Task<CategoryDto> Handle(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var name = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Tên danh mục không được để trống.");

            var isExists = await _categoryRepository.ExistsByNameAsync(
                name,
                null,
                cancellationToken);

            if (isExists)
                throw new Exception("Tên danh mục đã tồn tại.");

            var category = new Domain.Entities.Category
            {
                Name = name,
                Code = request.Code?.Trim(),
                Slug = request.Slug?.Trim(),
                IsActive = request.IsActive,
                SortOrder = request.SortOrder
            };

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.SaveChangesAsync(cancellationToken);

            await _categoryService.ClearCategoryCacheAsync(cancellationToken);
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Code = category.Code,
                IsActive = category.IsActive,
                SortOrder = category.SortOrder
            };
        }
    }


}
