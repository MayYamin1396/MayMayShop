using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetAllProductListBuyerResponse
    {
        public List<GetAllProductListMainCategoryBuyer> MainCategory {get;set;} 
    }
    public class GetAllProductListMainCategoryBuyer
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public List<GetAllProductListBuyer> ProductListBuyers{get;set;}
    }
    public class GetAllProductListBuyer{
        public int ProductId {get;set;}
        public string Url {get;set;}
        public string Name {get;set;}        
        public double OriginalPrice {get;set;}
        public double PromotePrice {get;set;}
        public int PromotePercent { get; set; }
    }
}