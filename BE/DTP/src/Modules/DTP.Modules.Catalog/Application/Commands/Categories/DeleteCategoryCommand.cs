using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Commands.Category
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteCategoryCommand(Guid id)
        {
            Id = id;
        }
    }


    public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryService _categoryService;

        public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, ICategoryService categoryService)
        {
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
        }

        public async Task<bool> Handle(
            DeleteCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (category == null)
                throw new Exception("Không tìm thấy danh mục.");

            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            await _categoryService.ClearCategoryCacheAsync(cancellationToken);
            return true;
        }
    }
}
