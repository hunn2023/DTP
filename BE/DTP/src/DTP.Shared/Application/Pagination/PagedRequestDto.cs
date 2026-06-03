using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Pagination
{
    public class PagedRequestDto
    {
        private const int MaxPageSize = 200;

        private int _pageIndex = 1;
        private int _pageSize = 20;

        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value <= 0 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                {
                    _pageSize = 20;
                    return;
                }

                _pageSize = value > MaxPageSize ? MaxPageSize : value;
            }
        }

        public int Skip => (PageIndex - 1) * PageSize;
    }
}
