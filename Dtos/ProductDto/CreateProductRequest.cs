using System;
using System.Collections.Generic;
using MayMayShop.API.Models;
using Microsoft.AspNetCore.Http;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class CreateProductRequest
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }
        public double Price { get; set; }
        public int BrandId {get;set;}
        public List<Tag> TagsList {get;set;}
        public int Promotion {get;set;}
        public ProductClipRequest ProductClip {get;set;}
        public List<ImageRequest> ImageList { get; set; }        
        public List<Sku> Sku { get; set; }
    }
    public class Sku
    {
        public int SkuId { get; set; }
        public int Qty { get; set; }
        public double Price {get;set;}
    }    
    public class ImageRequest{
        public int ImageId {get;set;}
        public string ImageContent { get; set; }
        public string Extension { get; set; }
        public string Action {get;set;}
    }
    public class ProductClipRequest{
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string ClipPath { get; set; }

        public int SeqNo { get; set; }
    }
}