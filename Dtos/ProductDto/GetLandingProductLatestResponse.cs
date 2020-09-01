using System;
namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetLandingProductLatestResponse
    {
        public int ProductId {get;set;}
        public string Url {get;set;}
        public string Name {get;set;}        
        public double OriginalPrice {get;set;}
        public double? PromotePrice {get;set;}
        public int PromotePercent { get; set; }
        public DateTime? CreatedDate {get;set;}
    }
}