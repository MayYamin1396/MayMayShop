using System.Collections.Generic;
using MayMayShop.API.Dtos.ProductDto;

namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class CreateMultipleBannerRequest{
        public List<BannerInfo> Banners {get;set;}
    }
    public class BannerInfo
    {
        public string Name {get;set;}
        public ImageRequest ImageRequest{get;set;}
        public int BannerLinkId {get;set;}
        public int BannerType {get;set;}
    }
}