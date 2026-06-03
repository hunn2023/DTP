using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;

namespace DTP.Modules.Catalog.Application.Commands.Category
{
    public class UpdateCategoryCommand : IRequest<CategoryDto>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Code { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int SortOrder { get; set; }
    }

    public class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryService _categoryService;

        public UpdateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ICategoryService categoryService)
        {
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
        }

        public async Task<CategoryDto> Handle(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (category == null)
                throw new Exception("Không tìm thấy danh mục.");

            var name = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Tên danh mục không được để trống.");

            var isExists = await _categoryRepository.ExistsByNameAsync(
                name,
                request.Id,
                cancellationToken);

            if (isExists)
                throw new Exception("Tên danh mục đã tồn tại.");

            category.Name = name;
            category.Code = request.Code?.Trim();
            //category.Description = request.Description?.Trim();
            category.IsActive = request.IsActive;
            category.SortOrder = request.SortOrder;
            //category.UpdatedAt = DateTime.UtcNow;

            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            await _categoryService.ClearCategoryCacheAsync(cancellationToken);

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                IsActive = category.IsActive,
                SortOrder = category.SortOrder
            };
        }
    }
}
