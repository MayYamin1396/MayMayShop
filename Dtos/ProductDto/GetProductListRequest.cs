using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetProductListRequest
    {
        [Required]
        public int Filter {get;set;}
        public string ProductStatus{get;set;}
        public string SearchText {get;set;}
        public int Count {get;set;}
        public int ProductCategoryId {get;set;}
        public int[] TagIDs {get;set;} 
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