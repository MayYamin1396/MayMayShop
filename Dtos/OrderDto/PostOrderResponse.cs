using MayMayShop.API.Dtos.GatewayDto;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.OrderDto
{
   public class PostOrderResponse : ResponseStatus
    {
        public int OrderId { get; set; }

        public int? Timestamp { get; set; }

        public string NonceStr { get; set; }

        public KBZPrecreateResponse Precreate { get; set; }
        
        public List<ProductIssues> ProductIssues{get;set;}
    }
    public class ProductIssues{
        public int ProductId {get;set;}
        public int SkuId {get;set;}
        public string ProductName {get;set;}
        public string Action {get;set;}
        public int Qty {get;set;}
        public string Reason {get;set;}
    }
}