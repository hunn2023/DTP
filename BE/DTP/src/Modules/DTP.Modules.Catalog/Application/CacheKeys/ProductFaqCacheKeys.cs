using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.CacheKeys
{
    public static class ProductFaqCacheKeys
    {
        private const string Prefix = "catalog:product-faqs";

        public static string ById(Guid id)
        {
            return $"{Prefix}:id:{id:N}";
        }

        public static string ByProductId(Guid productId, bool onlyActive)
        {
            return $"{Prefix}:product:{productId:N}:active:{onlyActive}";
        }

        public static string PrefixAll()
        {
            return Prefix;
        }
    }
}
