using DTP.Modules.Customer.Application.Abstractions.Repositories;
using DTP.Modules.Customer.Application.Abstractions.Services;
using DTP.Modules.Customer.Application.Services;
using DTP.Modules.Customer.Infrastructure.Persistence;
using DTP.Modules.Customer.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.SqlServer;

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
