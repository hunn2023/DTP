using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.CacheKeys
{
    public static class PhoneCardCacheKeys
    {
        public const string Prefix = "catalog:phonecards:";
        public const string ListPrefix = Prefix + "list:";
        public const string DetailPrefix = Prefix + "detail:";

           
        public const string PublicPrefix = "catalog:phone-cards:public";

        public static string PublicPaged(
            Guid? providerId,
            decimal? minFaceValue,
            decimal? maxFaceValue,
            int pageIndex,
            int pageSize)
        {
            return $"{PublicPrefix}:list:" +
                   $"provider:{providerId?.ToString() ?? "all"}:" +
                   $"minFace:{minFaceValue?.ToString() ?? "all"}:" +
                   $"maxFace:{maxFaceValue?.ToString() ?? "all"}:" +
                   $"page:{pageIndex}:size:{pageSize}";
        }

        public static string PublicBySlug(string slug)
            => $"{PublicPrefix}:slug:{slug}";

        public static string List(
            string? keyword,
            Guid? productId,
            Guid? countryId,
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
                   $"provider={providerId}:" +
                   $"active={isActive}:" +
                   $"page={page}:" +
                   $"size={pageSize}";
        }

        public static string Detail(Guid id)
            => DetailPrefix + id;
    }
}
