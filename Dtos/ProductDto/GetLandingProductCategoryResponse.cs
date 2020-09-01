using System.Collections.Generic;
namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetLandingProductCategoryResponse
    {
     public int MainCategoryId {get;set;}
     public string MainCategoryName {get;set;}
     public string Url {get;set;}
     public List<LandingProductCategory> LandingProductCategory {get;set;}
    }
    public class LandingProductCategory 
    {
        public int ProductId {get;set;}
        public string Url {get;set;}
        public string Name {get;set;}        
        public double OriginalPrice {get;set;}
        public double PromotePrice {get;set;}
        public int PromotePercent { get; set; }
        public int SubCategoryId {get;set;}
        public string SubCategoryName {get;set;}
    }
}