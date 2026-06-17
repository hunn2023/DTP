using DTP.Modules.Customer.Application.Abstractions.Repositories;
using DTP.Modules.Customer.Application.Abstractions.Services;
using DTP.Modules.Customer.Application.Features.Admin.Customers.Queries;
using DTP.Modules.Customer.Infrastructure.Repositories;
using DTP.Modules.Customer.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DTP.Modules.Customer
{
    public static class CustomerModule
    {
        public static IServiceCollection AddCustomerModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetAdminCustomersQuery).Assembly);
            });

            services.AddScoped<IAdminCustomerRepository, AdminCustomerRepository>();
            services.AddScoped<IAdminCustomerService, AdminCustomerService>();

            return services;
        }
    }
}
