using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.CacheKeys
{
    public static class EsimPackageCacheKeys
    {
        public const string PublicPrefix = "catalog:esim-packages:public";
        public const string Prefix = "catalog:esim:";
        public const string ListPrefix = Prefix + "list:";
        public const string DetailPrefix = Prefix + "detail:";

        public static string PublicPaged(
          Guid? countryId,
          Guid? carrierId,
          bool? isUnlimited,
          int? validityDays,
          int pageIndex,
          int pageSize)
        {
            return $"catalog:esim-packages:" +
                   $"country:{countryId?.ToString() ?? "all"}:" +
                   $"carrier:{carrierId?.ToString() ?? "all"}:" +
                   $"unlimited:{isUnlimited?.ToString() ?? "all"}:" +
                   $"validity:{validityDays?.ToString() ?? "all"}:" +
                   $"page:{pageIndex}:size:{pageSize}";
        }

        public static string PublicBySlug(string slug)
            => $"catalog:esim-package:{slug}";

        public static string List(
            string? keyword,
            Guid? productId,
            Guid? countryId,
            Guid? carrierId,
            Guid? providerId,
            bool? isActive,
            int page,
            int pageSize)
        {
            keyword = string.IsNullOrWhiteSpace(keyword)
                ? "all"
                : keyword.Trim().ToLower();

            return ListPrefix +
                   $"keyword={keyword}:" +
                   $"product={productId}:" +
                   $"country={countryId}:" +
                   $"carrier={carrierId}:" +
                   $"provider={providerId}:" +
                   $"active={isActive}:" +
                   $"page={page}:" +
                   $"size={pageSize}";
        }

        public static string Detail(Guid id)
            => DetailPrefix + id;
    }
}
