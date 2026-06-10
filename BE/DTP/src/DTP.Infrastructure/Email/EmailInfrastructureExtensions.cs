using DTP.Shared.Application.Emails;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Infrastructure.Email
{
    public static class EmailInfrastructureExtensions
    {
        public static IServiceCollection AddEmailInfrastructure(
            this IServiceCollection services)
        {
            services.AddScoped<IEmailSender, SmtpEmailSender>();

            return services;
        }
    }
}
