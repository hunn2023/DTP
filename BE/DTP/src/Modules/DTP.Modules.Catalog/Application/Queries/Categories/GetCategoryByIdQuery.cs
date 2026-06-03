using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Category
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto?>
    {
        public Guid Id { get; set; }

        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryService _categoryService;

        public GetCategoryByIdQueryHandler(
            ICategoryRepository categoryRepository,
            ICategoryService categoryService)
        {
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
        }

        public async Task<CategoryDto?> Handle(
            GetCategoryByIdQuery request,
            CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (category == null)
                return null;

            return new CategoryDto()
            {
            };
        }
    }
}
