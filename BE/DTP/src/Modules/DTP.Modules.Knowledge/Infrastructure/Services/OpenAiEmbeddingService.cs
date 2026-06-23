using DTP.Modules.Knowledge.Application.Abstractions;
using DTP.Modules.Knowledge.Application.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Services
{
    public class OpenAiEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly KnowledgeOpenAiOptions _options;

        public OpenAiEmbeddingService(
            HttpClient httpClient,
            IOptions<KnowledgeOpenAiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        }

        public async Task<float[]> CreateEmbeddingAsync(
            string input,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Array.Empty<float>();

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                throw new InvalidOperationException("Knowledge:OpenAI:ApiKey is missing.");

            var url = $"{_options.BaseUrl.TrimEnd('/')}/v1/embeddings";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            var body = new
            {
                model = _options.EmbeddingModel,
                input
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"OpenAI embedding failed. Status: {(int)response.StatusCode}. Body: {raw}");
            }

            var result = JsonSerializer.Deserialize<OpenAiEmbeddingResponse>(
                raw,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var embedding = result?.Data?.FirstOrDefault()?.Embedding;

            if (embedding == null || embedding.Length == 0)
                throw new InvalidOperationException("OpenAI embedding response is empty.");

            return embedding;
        }

        private class OpenAiEmbeddingResponse
        {
            public List<OpenAiEmbeddingData> Data { get; set; } = new();
        }

        private class OpenAiEmbeddingData
        {
            public float[] Embedding { get; set; } = Array.Empty<float>();
        }
    }
}
