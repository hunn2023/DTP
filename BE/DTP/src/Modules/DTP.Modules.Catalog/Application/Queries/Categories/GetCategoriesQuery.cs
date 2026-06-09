using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Category
{
    public class GetCategoriesQuery : IRequest<Result<PagedResultDto<CategoryDto>>>
    {
        public string? Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; }
    }

    public class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<PagedResultDto<CategoryDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;


        public GetCategoriesQueryHandler(
            ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Result<PagedResultDto<CategoryDto>>> Handle(
            GetCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetPublicPagedAsync(request.PageIndex, request.PageSize, cancellationToken);
            return Result<PagedResultDto<CategoryDto>>.Success(categories);
        }
    }
}
