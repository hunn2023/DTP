using DTP.Modules.Knowledge.Application.Abstractions;
using DTP.Modules.Knowledge.Application.Options;
using DTP.Modules.Knowledge.Infrastructure.Persistence;
using DTP.Modules.Knowledge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddKnowledgeModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.Configure<KnowledgeOptions>(
                configuration.GetSection("Knowledge"));

            services.Configure<KnowledgeOpenAiOptions>(
                configuration.GetSection("Knowledge:OpenAI"));

            services.AddDbContext<KnowledgeDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IKnowledgeDbContext>(sp =>
                sp.GetRequiredService<KnowledgeDbContext>());

            services.AddHttpClient<IEmbeddingService, OpenAiEmbeddingService>();

            services.AddScoped<ITextChunkerService, TextChunkerService>();
            services.AddScoped<IKnowledgeIndexerService, KnowledgeIndexerService>();
            services.AddScoped<IKnowledgeSearchService, KnowledgeSearchService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            return services;
        }
    }
}
