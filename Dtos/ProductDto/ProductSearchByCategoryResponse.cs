using System;
using System.Collections.Generic;
using MayMayShop.API.Models;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class ProductSearchByCategoryResponse:ResponseStatus
    {
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public List<ProductCtgry> ProductList{get;set;}
        public int Count {get;set;}
    }

    public class ProductCtgry{
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PriceId {get;set;}
        public double OriginalPrice { get; set; }
        public double PromotePrice { get; set; }
        public int PromotePercent { get; set; }
        public int SubCategoryId {get;set;}
        public string SubCategoryName {get;set;}
        public DateTime? CreatedDate{get;set;}
        public List<productImg> ProductImage {get;set;}
    }

    public class productImg{
        public int Id { get; set; }
        public string Url { get; set; }
        public bool isMain { get; set; }
    }
}