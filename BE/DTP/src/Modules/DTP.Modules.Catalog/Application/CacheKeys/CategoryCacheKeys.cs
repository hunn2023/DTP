using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.CacheKeys
{

    public static class CategoryCacheKeys
    {
        public const string Prefix = "catalog:categories:";

        public static string PublicActivePaged(int pageIndex, int pageSize)
      => $"catalog:categories:public:active:page:{pageIndex}:size:{pageSize}";

        public const string ActiveList = Prefix + "active";

        public const string Tree = Prefix + "tree";

        public static string Detail(Guid id)
            => Prefix + $"detail:{id}";

        public static string List(string? keyword)
            => Prefix + $"list:keyword={keyword}";
    }
}
