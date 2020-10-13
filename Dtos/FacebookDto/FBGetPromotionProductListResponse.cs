namespace MayMayShop.API.Dtos.FacebookDto
{
    public class FBGetPromotionProductListResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double OriginalPrice { get; set; }
        public double PromotePrice{get;set;}
        public int PromotePercent{get;set;}
        public string Url { get; set; }
    }
}