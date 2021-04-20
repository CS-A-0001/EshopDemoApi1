using System.Collections.Generic;

namespace CatalogDemo.API.Versions.V2.Models
{    
    public class PaginatedItems<TEntity> where TEntity : class
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public long Count { get; private set; }
        public IEnumerable<TEntity> Data { get; private set; }

        public PaginatedItems(int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }
    }
}
