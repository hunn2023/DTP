
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Catalog.Application.Commands.Category
{
    public class DeleteCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public DeleteCategoryCommand(Guid id)
        {
            Id = id;
        }
    }


    public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly ICategoryService _categoryService;

        public DeleteCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result> Handle(
            DeleteCategoryCommand request,
            CancellationToken cancellationToken)
        {
            return await _categoryService.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
