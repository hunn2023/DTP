using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.CacheKeys
{
    public static class ProductCacheKeys
    {
        public const string Prefix = "catalog:products:";

        public const string ListPrefix = Prefix + "list:";
        public const string DetailPrefix = Prefix + "detail:";

        public static string PublicPaged(
            string? keyword,
            Guid? categoryId,
            Guid? countryId,
            Guid? carrierId,
            int pageIndex,
            int pageSize)
        {
            return $"catalog:products:public:list:" +
                   $"keyword:{keyword ?? "all"}:" +
                   $"category:{categoryId?.ToString() ?? "all"}:" +
                   $"country:{countryId?.ToString() ?? "all"}:" +
                   $"carrier:{carrierId?.ToString() ?? "all"}:" +
                   $"page:{pageIndex}:size:{pageSize}";
        }

        public static string PublicBySlug(string slug)
            => $"catalog:products:public:slug:{slug}";


        public static string List(
            string? keyword,
            Guid? categoryId,
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
                   $"category={categoryId}:" +
                   $"country={countryId}:" +
                   $"carrier={carrierId}:" +
                   $"provider={providerId}:" +
                   $"active={isActive}:" +
                   $"page={page}:" +
                   $"size={pageSize}";
        }

        public static string Detail(Guid productId)
            => DetailPrefix + productId;
    }
}
