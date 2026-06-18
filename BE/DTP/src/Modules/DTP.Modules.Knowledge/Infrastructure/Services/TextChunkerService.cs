using DTP.Modules.Knowledge.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Services
{
    public class TextChunkerService : ITextChunkerService
    {
        public IReadOnlyList<string> Split(string text, int maxChars, int overlapChars)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<string>();

            text = NormalizeText(text);

            if (text.Length <= maxChars)
                return new List<string> { text };

            var chunks = new List<string>();
            var start = 0;

            while (start < text.Length)
            {
                var length = Math.Min(maxChars, text.Length - start);
                var chunk = text.Substring(start, length);

                var lastBreak = chunk.LastIndexOf('\n');

                if (lastBreak > maxChars * 0.6)
                {
                    chunk = chunk[..lastBreak];
                }

                chunk = chunk.Trim();

                if (!string.IsNullOrWhiteSpace(chunk))
                    chunks.Add(chunk);

                start += Math.Max(1, chunk.Length - overlapChars);
            }

            return chunks;
        }

        private static string NormalizeText(string text)
        {
            text = Regex.Replace(text, "<.*?>", " ");
            text = text.Replace("&nbsp;", " ");
            text = text.Replace("&amp;", "&");
            text = Regex.Replace(text, @"[ \t]+", " ");
            text = Regex.Replace(text, @"\n{3,}", "\n\n");

            return text.Trim();
        }
    }
}
