using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetOrderListByProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        public int OrderCount { get; set; }
        public int TotalQty { get; set; }
        public double OriginalPrice { get; set; } 
        public double PromotePrice { get; set; }  
        public double PromotePercent { get; set; }        
        public DateTime OrderDate { get; set; }
        // public string SkuValue { get; set; }
        public List<string> UserImage { get; set; }
    }
    public class OrderProductResponse
    {
        public string ProductName { get; set; }
        public int OrderCount { get; set; }
        public int TotalQty { get; set; }
        public int ProductId { get; set; }
    }
}
