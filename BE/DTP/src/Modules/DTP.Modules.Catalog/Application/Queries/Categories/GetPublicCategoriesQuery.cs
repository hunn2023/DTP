using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Categories
{
    public class GetPublicCategoriesQuery
     : IRequest<PagedResultDto<CategoryDto>>
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetPublicCategoriesQueryHandler
    : IRequestHandler<GetPublicCategoriesQuery, PagedResultDto<CategoryDto>>
    {
        private readonly ICategoryService _categoryService;

        public GetPublicCategoriesQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<PagedResultDto<CategoryDto>> Handle(
            GetPublicCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            return await _categoryService.GetPublicAsync(
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
