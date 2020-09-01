using System;
using System.Collections.Generic;
using MayMayShop.API.Dtos.ProductDto;
using MayMayShop.API.Models;

namespace MayMayShop.API.Dtos.MembershipDto
{
    public class GetRewardProductDetailResponse : ResponseStatus
    {
        public int ProductId {get;set;}
        public string Name {get;set;}
        public DateTime StartDate {get;set;}
        public DateTime EndDate {get;set;}
        public int Point {get;set;}
        public double OriginalPrice {get;set;}
        public double FixedAmount {get;set;}
        public int RewardPercent {get;set;}
        public double RewardAmount {get;set;}
        public string Description {get;set;}
        public int MyOwnPoint {get;set;}
        public int Qty { get; set; }
        public List<GetPrdouctDetailCategoryResponse> ProductCategory {get;set;}
        public string[] ProductTag {get;set;}
        public string[] ProductImage {get;set;}
        public List<GetProductDetailVariant> Variant { get; set; }
        public List<GetProductDetailSkuValue> SkuValue { get; set; }
        public List<GetVariantValueResponse> VariantValues { get; set; }

    }
}