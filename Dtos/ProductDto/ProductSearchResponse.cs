using System;
using System.Collections.Generic;
using MayMayShop.API.Models;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class ProductSearchResponse:ResponseStatus
    {
        public List<ProductInfo> ProductList{get;set;}
        public int Count {get;set;}
    }

    public class ProductInfo {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public double OriginalPrice { get; set; }
        public double PromotePrice {get;set;}
        public int PromotePercent {get;set;}
        public int Qty { get; set; }
        public DateTime? CreatedDate{get;set;}
        public int OrderCount { get; set; }
        public string Url {get;set;}
    }
}