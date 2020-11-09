using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetProductListResponse
    {
         public int Count { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Qty{get;set;}
        public string Sku{get;set;}
        public string Url{get;set;}
        public double OriginalPrice {get;set;}
        public double PromotePrice {get;set;}
        public int PromotePercent { get; set; }
        public double RewardAmount {get;set;}
        public int Point {get;set;}
        public double FixedAmount {get;set;}
        public int RewardPercent {get;set;}
        public DateTime RewardStartDate {get;set;}
        public DateTime RewardEndDate {get;set;}
        public int ProductRewardId {get;set;}
        public string ProductStatus {get;set;}
    }
}