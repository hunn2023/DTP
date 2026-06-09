using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Shared.Application;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Auth.Infrastructure.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly AuthDbContext _context;

        public PermissionService(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<PermissionDto>>> GetListAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _context.Permissions
                .AsNoTracking()
                .OrderBy(x => x.Module)
                .ThenBy(x => x.Code)
                .Select(x => new PermissionDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Module = x.Module,
                    Description = x.Description
                })
                .ToListAsync(cancellationToken);

            return Result<List<PermissionDto>>.Success(result);
        }

        public async Task<Result<Dictionary<string, List<PermissionDto>>>> GetByModuleAsync(
            CancellationToken cancellationToken = default)
        {
            var permissionsResult = await GetListAsync(cancellationToken);


            var permissions = permissionsResult.Data ?? new List<PermissionDto>();
            var result = permissions
                .GroupBy(x => x.Module)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());

            return Result<Dictionary<string, List<PermissionDto>>>.Success(result);
        }
    }
}
