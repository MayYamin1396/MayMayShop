using MayMayShop.API.Dtos.ProductDto;

namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class CreateBannerRequest
    {
        public string Name {get;set;}
        public ImageRequest ImageRequest{get;set;}
        public int BannerLinkId {get;set;}
        public int BannerType {get;set;}
    }
}