using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.CacheKeys
{
    public static class CarrierCacheKeys
    {
        public const string Prefix = "catalog:carriers:";

        public const string ActiveList = Prefix + "active";

        public static string PublicActivePaged(int pageIndex, int pageSize)
      => $"catalog:carriers:public:active:page:{pageIndex}:size:{pageSize}";

        public static string Detail(Guid id)
            => Prefix + $"detail:{id}";

        public static string List(string? keyword)
            => Prefix + $"list:keyword={keyword}";
    }
}
