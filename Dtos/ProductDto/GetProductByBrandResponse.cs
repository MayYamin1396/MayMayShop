using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetProductByBrandResponse
    {
        public string BrandName {get;set;}

        public string BrandLogo {get;set;}

        public string Url {get;set;}

        public List<BrandProduct> Products {get;set;}
    }
    public class BrandProduct {
        public int ProductId {get;set;}
        public string Url {get;set;}
        public string Name {get;set;}        
        public double OriginalPrice {get;set;}
        public double? PromotePrice {get;set;}
        public int PromotePercent { get; set; }
        public DateTime? CreatedDate {get;set;}
    }
}