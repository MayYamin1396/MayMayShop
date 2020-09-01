using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class ProductSearchRequest
    {
        [Required]
        public int SearchType {get;set;}
        public string ProductName{ get; set; }        
        public int ProductCategoryId{get;set;}
        public int[] tagIds { get; set; }
        public int Choose {get;set;}
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