namespace DTP.Modules.Customer.Application.Common
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = new List<T>();

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages => PageSize <= 0
            ? 0
            : (int)Math.Ceiling(TotalItems / (double)PageSize);
    }
}
