using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.CacheKeys
{

    public static class CountryCacheKeys
    {
        public const string Prefix = "catalog:countries:";

        public static string PublicActivePaged(int pageIndex, int pageSize)
     => $"catalog:countries:public:active:page:{pageIndex}:size:{pageSize}";

        public const string ActiveList = Prefix + "active";

        public static string Detail(Guid id)
            => Prefix + $"detail:{id}";

        public static string List(string? keyword, int page, int pageSize)
            => Prefix + $"list:keyword={keyword}:page={page}:size={pageSize}";
    }
}
