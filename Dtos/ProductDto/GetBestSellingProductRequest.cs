using System;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetBestSellingProductRequest
    {
        // public DateTime FromDate {get;set;}
        // public DateTime ToDate {get;set;}
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
    }
}