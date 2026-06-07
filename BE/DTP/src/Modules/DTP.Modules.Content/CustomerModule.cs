using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.Services;
using DTP.Modules.Content.Infrastructure.Persistence;
using DTP.Modules.Content.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DTP.Modules.Customer
{
    public static class CustomerModule
    {
        public static IServiceCollection AddCustomerModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CustomerDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerAddressRepository, CustomerAddressRepository>();
            services.AddScoped<ICustomerUnitOfWork, CustomerUnitOfWork>();

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ICustomerAddressService, CustomerAddressService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CustomerModule).Assembly);
            });

            return services;
        }
    }
}
