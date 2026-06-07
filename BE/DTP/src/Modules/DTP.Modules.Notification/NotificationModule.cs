using DTP.Modules.Notification.Application.Abstractions.Repositories;
using DTP.Modules.Notification.Application.Abstractions.Services;
using DTP.Modules.Notification.Infrastructure.Persistence;
using DTP.Modules.Notification.Infrastructure.Repositories;
using DTP.Modules.Notification.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DTP.Modules.Notification
{
    public static class NotificationModule
    {
        public static IServiceCollection AddNotificationModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<NotificationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.Configure<EmailNotificationSettings>(
                configuration.GetSection("Notification:Email"));

            services.AddScoped<INotificationUnitOfWork, NotificationUnitOfWork>();

            services.AddScoped<INotificationMessageRepository, NotificationMessageRepository>();
            services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
            services.AddScoped<INotificationDeliveryLogRepository, NotificationDeliveryLogRepository>();

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailNotificationSender, EmailNotificationSender>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(NotificationModule).Assembly);
            });

            return services;
        }
    }
}
