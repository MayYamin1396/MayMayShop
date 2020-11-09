using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class PopulateSkuRequest
    {
        public int ProductCategoryId { get; set; }
        public List<Options> Options { get; set; }
        public List<SkuImage> SkuImages{get;set;}
    }

    public class Options
    {
        public int VariantId { get; set; }
        public List<string> OptionValue  { get; set; }        
    }

    public class SkuImage{
        public int VariantId {get;set;}
        public string OptionValue {get;set;}
        public List<ImageSkuRequest> ImageList{get;set;}
    }

    public class ImageSkuRequest{
        public string Url {get;set;}
        public string Thumbnail{get;set;}
        public string ImageContent { get; set; }
        public string Extension { get; set; }
        public int SeqNo {get;set;}
    }
    
}