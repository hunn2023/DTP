using DTP.Modules.Content.Application.Abstractions;
using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Infrastructure.Persistence;
using DTP.Modules.Content.Infrastructure.Repositories;
using DTP.Modules.Content.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DTP.Modules.Customer
{
    public static class ContentModule
    {
        public static IServiceCollection AddContentModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ContentDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql =>
                    {
                        sql.MigrationsAssembly(typeof(ContentDbContext).Assembly.FullName);
                    });
            });

            services.AddScoped<IContentUnitOfWork, ContentUnitOfWork>();

            services.AddScoped<IContentPageRepository, ContentPageRepository>();
            services.AddScoped<IContentArticleRepository, ContentArticleRepository>();
            services.AddScoped<IContentBannerRepository, ContentBannerRepository>();
            services.AddScoped<IContentFaqRepository, ContentFaqRepository>();
            services.AddScoped<ISeoMetadataRepository, SeoMetadataRepository>();

            services.AddScoped<IContentPageService, ContentPageService>();
            services.AddScoped<IContentArticleService, ContentArticleService>();
            services.AddScoped<IContentBannerService, ContentBannerService>();
            services.AddScoped<IContentFaqService, ContentFaqService>();
            services.AddScoped<ISeoMetadataService, SeoMetadataService>();


            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ContentModule).Assembly);
            });

            return services;
        }
    }
}
