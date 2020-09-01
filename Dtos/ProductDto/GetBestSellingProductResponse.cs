namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetBestSellingProductResponse
    {
        public int Id { get; set; }    
        public string Name { get; set; }    
        public double OriginalPrice { get; set; }
        public double PromotePrice { get; set; }
        public int PromotePercent { get; set; }
        public string Url { get; set; }
        public int OrderCount{get;set;}
    }
}