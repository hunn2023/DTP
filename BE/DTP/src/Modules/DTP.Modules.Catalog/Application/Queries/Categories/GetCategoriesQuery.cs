using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Category
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
        public string? Keyword { get; set; }
    }

    public class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryService _categoryService;

        public GetCategoriesQueryHandler(
            ICategoryRepository categoryRepository,
            ICategoryService categoryService)
        {
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
        }

        public async Task<List<CategoryDto>> Handle(
            GetCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetListAsync(
                request.Keyword,
                cancellationToken);

            return new List<CategoryDto>();
        }
    }
}
