using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.Commands;
using DTP.Modules.Chatbot.Application.Services;
using DTP.Modules.Chatbot.Infrastructure.Clients;
using DTP.Modules.Chatbot.Infrastructure.Options;
using DTP.Modules.Chatbot.Infrastructure.Readers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot
{
    public static class ChatbotModule
    {
        public static IServiceCollection AddChatbotModule(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.Configure<OpenAiChatbotOptions>(
                configuration.GetSection("Chatbot:OpenAI"));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(
                    typeof(SendChatMessageCommand).Assembly);
            });

            services.AddScoped<IChatbotService, ChatbotService>();
            services.AddScoped<IChatbotCatalogReader, ChatbotCatalogReader>();
            services.AddHttpContextAccessor();
            services.AddHttpClient<IChatbotAiClient, OpenAiChatbotClient>((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<OpenAiChatbotOptions>>().Value;

                httpClient.BaseAddress = new Uri(
                    string.IsNullOrWhiteSpace(options.BaseUrl)
                        ? "https://api.openai.com"
                        : options.BaseUrl.TrimEnd('/'));

                httpClient.Timeout = TimeSpan.FromSeconds(60);
            });

            return services;
        }
    }
}
