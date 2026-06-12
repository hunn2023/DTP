using DTP.Infrastructure.Email;
using DTP.Modules.Audit;
using DTP.Modules.Auth;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Modules.Auth.Infrastructure.Seed;
using DTP.Modules.Catalog;
using DTP.Modules.Delivery;
using DTP.Modules.Ordering;
using DTP.Modules.Payment;
using DTP.Modules.Report;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

namespace DTP.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DtpCors", policy =>
                {
                    policy
                        .WithOrigins(
                            "https://dtp-admin-c5dd3.web.app",
                            "https://dtpweb-94f64.web.app",
                            "http://localhost:5173",
                            "http://localhost:3000"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services
                .AddControllers()
                .AddApplicationPart(typeof(CatalogModule).Assembly)
                .AddApplicationPart(typeof(AuthModule).Assembly);

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto;

                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });


            builder.Services.AddSwaggerGen(options =>
            {
                        options.SwaggerDoc("v1", new OpenApiInfo
                        {
                            Title = "DTP API",
                            Version = "v1"
                        });

                        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            Name = "Authorization",
                            Type = SecuritySchemeType.Http,
                            Scheme = "Bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Description = "Nhập token dạng: Bearer {token}"
                        });

                        options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
                    });

            var jwtSecret = builder.Configuration["Jwt:Secret"];

            if (string.IsNullOrWhiteSpace(jwtSecret))
            {
                throw new InvalidOperationException("Missing configuration: Jwt:Secret");
            }

        

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSecret))
                    };
                });

            builder.Services.AddAuthorization();
            builder.Services.AddAuthModule(builder.Configuration);
            builder.Services.AddCatalogModule(builder.Configuration);
            builder.Services.AddAuditModule(builder.Configuration);

            builder.Services.AddOrderingModule(builder.Configuration);
            builder.Services.AddPaymentModule(builder.Configuration);
            builder.Services.AddDeliveryModule(builder.Configuration);
            builder.Services.AddReportModule(builder.Configuration);
            builder.Services.AddEmailInfrastructure();


            builder.Services.AddHttpContextAccessor();
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Bạn thao tác quá nhanh. Vui lòng thử lại sau.",
                        statusCode = 429
                    }, cancellationToken);
                };

                options.AddPolicy("auth-login", httpContext =>
                {
                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth-login:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("auth-register", httpContext =>
                {
                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth-register:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromHours(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("auth-otp", httpContext =>
                {
                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth-otp:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(10),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("auth-forgot-password", httpContext =>
                {
                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth-forgot-password:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(15),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("auth-refresh-token", httpContext =>
                {
                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth-refresh-token:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 20,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });



                //payment
                options.AddPolicy("payment-create-qr", httpContext =>
                {
                    var userId =
                        httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst("nameid")?.Value ??
                        httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                        "anonymous";

                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"payment-create-qr:{userId}:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(5),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("payment-read", httpContext =>
                {
                    var userId =
                        httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst("nameid")?.Value ??
                        httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                        "anonymous";

                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"payment-read:{userId}:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("payment-webhook", httpContext =>
                {
                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"payment-webhook:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });


                //delivery
                options.AddPolicy("delivery-process", httpContext =>
                {
                    var userId =
                        httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst("nameid")?.Value ??
                        httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                        "anonymous";

                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"delivery-process:{userId}:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(5),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("delivery-read", httpContext =>
                {
                    var userId =
                        httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst("nameid")?.Value ??
                        httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                        "anonymous";

                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"delivery-read:{userId}:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("delivery-admin", httpContext =>
                {
                    var userId =
                        httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst("nameid")?.Value ??
                        httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                        "anonymous";

                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"delivery-admin:{userId}:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 30,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/json";

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Bạn thao tác quá nhanh. Vui lòng thử lại sau.",
                        statusCode = 429
                    }, cancellationToken);
                };

                options.AddPolicy("ordering-create", httpContext =>
                {
                    var userId = httpContext.User.FindFirst("sub")?.Value
                                 ?? httpContext.User.FindFirst("nameid")?.Value
                                 ?? "anonymous";

                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"ordering-create:{userId}:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(5),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("ordering-read", httpContext =>
                {
                    var userId = httpContext.User.FindFirst("sub")?.Value
                                 ?? httpContext.User.FindFirst("nameid")?.Value
                                 ?? "anonymous";

                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"ordering-read:{userId}:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });

                options.AddPolicy("ordering-admin", httpContext =>
                {
                    var userId = httpContext.User.FindFirst("sub")?.Value
                                 ?? httpContext.User.FindFirst("nameid")?.Value
                                 ?? "anonymous";

                    var ip = GetClientIp(httpContext);

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"ordering-admin:{userId}:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 30,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });
            });

            static string GetClientIp(HttpContext context)
            {
                var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            }



            var app = builder.Build();

            app.UseSwagger();
            //using (var scope = app.Services.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            //    await AuthSeeder.SeedAdminAsync(dbContext);
            //}
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "DTP API v1");
            });

            app.UseRouting();
            app.UseCors("DtpCors");
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}