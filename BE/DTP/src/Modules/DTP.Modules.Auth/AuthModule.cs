using DTP.Infrastructure.Caching;
using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Modules.Auth.Infrastructure.Repositories;
using DTP.Modules.Auth.Infrastructure.Services;
using DTP.Shared.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;


namespace DTP.Modules.Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConnection = configuration["Redis:Connection"];

            if (string.IsNullOrWhiteSpace(redisConnection))
            {
                throw new Exception("Redis:Connection is missing.");
            }

            return ConnectionMultiplexer.Connect(redisConnection);
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPendingRegistrationRepository, PendingRegistrationRepository>();
        services.AddScoped<IAuthAttemptLogRepository, AuthAttemptLogRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRateLimitService, RateLimitService>();


        services.AddScoped<IAuthRateLimitService, AuthRateLimitService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AuthModule).Assembly);
        });

        return services;
    }
}