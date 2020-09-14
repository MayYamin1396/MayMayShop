using System;
using System.Collections.Generic;
using MayMayShop.API.Models;
using Microsoft.AspNetCore.Http;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class UpdateProductRequest
    {
        public int ProductId { get; set; }
        public string Name { get; set; }        
        public string Description { get; set; }
        public int PriceId {get;set;}
        public double Price { get; set; }

        public int BrandId {get;set;}
        public List<Tag> TagsList {get;set;}
        public int PromotionId {get;set;}
        public int Promotion {get;set;}
        public ProductClipRequest ProductClip {get;set;}
        public List<ImageRequest> ImageList { get; set; }  
        public List<Sku> Sku { get; set; }
        
    }
}