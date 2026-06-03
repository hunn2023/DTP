using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.CacheKeys
{
    public static class ProviderCacheKeys
    {
        public const string Prefix = "catalog:providers:";

        public const string ActiveList = Prefix + "active";

        public static string Detail(Guid id)
            => Prefix + $"detail:{id}";

        public static string List(string? keyword)
            => Prefix + $"list:keyword={keyword}";
    }
}
