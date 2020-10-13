using System.Collections.Generic;
using MayMayShop.API.Models;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetProductDetailResponse : ResponseStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PriceId {get;set;}
        public double Price { get; set; }
        public int PromoteId {get;set;}
        public double PromotePrice { get;set;}
        public int PromotePercent { get; set; }
        public int Qty { get; set; }
        public int ProductCategoryId {get;set;}
        public string SharedUrl {get;set;}
        // public List<ProductTag> ProductTag {get;set;}
        public List<Tag> TagsList {get;set;}
        public List<ProductImage> ProductImage {get;set;}
        public ProductPromotion ProductPromotion {get;set;}
        public ProductClip ProductClip {get;set;}        
        public List<GetPrdouctDetailCategoryResponse> ProductCategory { get; set; }
        public List<GetProductDetailVariant> Variant { get; set; }
        public List<GetProductDetailSkuValue> SkuValue { get; set; }
        public List<GetVariantValueResponse> VariantValues { get; set; }

    }
    public class GetPrdouctDetailCategoryResponse{
        public int ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public string Url {get;set;}
        public bool IsMainCategory {get;set;}
    }
 
    public class GetProductDetailVariant
    {
        public int VariantId { get; set; }

        public string Name { get; set; }
    }

    public class GetProductDetailSkuValue 
    {
        public int SkuId { get; set; }

        public string Value { get; set; }

        public int Qty { get; set; }
        public double OriginalPrice {get;set;}
        public double PromotePrice {get;set;}
        public int PromotePercent {get;set;}
        public double FixedAmount {get;set;}
        public int RewardPercent {get;set;}
        public double RewardAmount {get;set;}
        public int Point {get;set;}
    }
    // public class ProductDetailClip{
    //     public int ProductId { get; set; }

    //     public string Name { get; set; }

    //     public string ClipPath { get; set; }

    //     public int SeqNo { get; set; }
    // }
}