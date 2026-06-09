using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.Category
{
    public class GetCategoryByIdQuery : IRequest<Result<CategoryDto?>>
    {
        public Guid Id { get; set; }

        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto?>>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryByIdQueryHandler(
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<CategoryDto?>> Handle(
            GetCategoryByIdQuery request,
            CancellationToken cancellationToken)
        {

            return await _categoryService.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
