using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Auth.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly AuthDbContext _context;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            AuthDbContext context)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _context = context;
        }

        public async Task<Result<PagedResultDto<UserDto>>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;


            return Result<PagedResultDto<UserDto>>.Success(
                await _userRepository.GetPagedAsync(
                    keyword,
                    pageIndex,
                    pageSize,
                    cancellationToken));
        }

        public async Task<Result<UserDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id, cancellationToken);

            if (user == null)
                throw new Exception("Không tìm thấy user.");

            return Result<UserDto>.Success(MapUserDto(user));

        }

        public async Task<Result> CreateAsync(
            CreateUserDto request,
            CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLower();

            var exists = await _userRepository.ExistsByEmailAsync(
                email,
                null,
                cancellationToken);

            if (exists)
                return Result.Failure("Email đã tồn tại.");

            var user = new User
            {
                Email = email,
                Phone = request.Phone,
                FullName = request.FullName.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                EmailConfirmed = true,
                IsActive = true
            };

            if (request.RoleIds.Any())
            {
                foreach (var roleId in request.RoleIds.Distinct())
                {
                    var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

                    if (role == null)
                        return Result.Failure($"Role không tồn tại: {roleId}");

                    user.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    });
                }
            }

            await _userRepository.AddAsync(user, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            UpdateUserDto request,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (user == null)
                return Result.Failure("Không tìm thấy user.");

            user.FullName = request.FullName.Trim();
            user.Phone = request.Phone;
            user.AvatarUrl = request.AvatarUrl;
            user.IsActive = request.IsActive;

            _userRepository.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> LockAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (user == null)
                return Result.Failure("Không tìm thấy user.");

            user.Lock();

            _userRepository.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> UnlockAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (user == null)
                return Result.Failure("Không tìm thấy user.");

            user.Unlock();

            _userRepository.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> AssignRolesAsync(
            Guid userId,
            List<Guid> roleIds,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(
                userId,
                cancellationToken);

            if (user == null)
                return Result.Failure("Không tìm thấy user.");

            var distinctRoleIds = roleIds.Distinct().ToList();

            foreach (var roleId in distinctRoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

                if (role == null)
                    return Result.Failure($"Role không tồn tại: {roleId}");
            }

            var oldRoles = await _context.UserRoles
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

            _context.UserRoles.RemoveRange(oldRoles);

            foreach (var roleId in distinctRoleIds)
            {
                await _context.UserRoles.AddAsync(new UserRole
                {
                    UserId = userId,
                    RoleId = roleId
                }, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        private static UserDto MapUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Phone = user.Phone,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed
            };
        }
    }
}
