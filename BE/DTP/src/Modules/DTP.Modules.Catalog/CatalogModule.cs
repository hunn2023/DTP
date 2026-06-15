using Amazon.Runtime;
using Amazon.S3;
using DTP.Infrastructure.Caching;
using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Modules.Catalog.Infrastructure.Repositories;
using DTP.Modules.Catalog.Infrastructure.Services;
using DTP.Shared.Caching;
using DTP.Shared.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DTP.Modules.Catalog
{
    public static class CatalogModule
    {
        public static IServiceCollection AddCatalogModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            var redisConnection = configuration["Redis:Connection"];

            if (!string.IsNullOrWhiteSpace(redisConnection))
            {
                services.AddSingleton<IConnectionMultiplexer>(
                    ConnectionMultiplexer.Connect(redisConnection));

                services.AddScoped<ICacheService, RedisCacheService>();
            }

            //services.AddSingleton<IAmazonS3>(sp =>
            //{
            //    var settings = sp.GetRequiredService<IOptions<CloudflareR2Settings>>().Value;

            //    var credentials = new BasicAWSCredentials(
            //        settings.AccessKey,
            //        settings.SecretKey);

            //    var config = new AmazonS3Config
            //    {
            //        ServiceURL = settings.ServiceUrl,
            //        ForcePathStyle = true,
            //        AuthenticationRegion = "auto"
            //    };

            //    return new AmazonS3Client(credentials, config);
            //});



            // Cloudflare R2
            services.Configure<CloudflareR2Settings>(
                configuration.GetSection("CloudflareR2"));

            services.AddScoped<IFileStorageService, CloudflareR2StorageService>();




            services.AddScoped<ICatalogUnitOfWork, CatalogUnitOfWork>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICarrierRepository, CarrierRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
            services.AddScoped<IProductPriceRepository, ProductPriceRepository>();
            services.AddScoped<IEsimPackageRepository, EsimPackageRepository>();
            services.AddScoped<IPhoneCardRepository, PhoneCardRepository>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICarrierService, CarrierService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductPriceService, ProductPriceService>();
            services.AddScoped<IEsimPackageService, EsimPackageService>();
            services.AddScoped<IPhoneCardService, PhoneCardService>();
            services.AddScoped<IProductCacheInvalidator, ProductCacheInvalidator>();
            services.AddScoped<ICatalogProductTypeCacheInvalidator, CatalogProductTypeCacheInvalidator>();
            services.AddScoped<IProductAttributeService, ProductAttributeService>();
            services.AddScoped<IProductVariantFeatureRepository, ProductVariantFeatureRepository>();
            services.AddScoped<IProductVariantFeatureService, ProductVariantFeatureService>();

            services.AddScoped<IProductContentRepository, ProductContentRepository>();
            services.AddScoped<IProductFaqRepository, ProductFaqRepository>();

            services.AddScoped<IProductContentService, ProductContentService>();
            services.AddScoped<IProductFaqService, ProductFaqService>();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CatalogModule).Assembly);
            });

            return services;
        }
    }
}