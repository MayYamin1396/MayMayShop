using System.Collections.Generic;
using System.Threading.Tasks;
using MayMayShop.API.Dtos.FacebookDto;

namespace MayMayShop.API.Interfaces.Repos
{
    public interface IFacebookRepository
    {
         public Task<List<FBGetMainCategoryResponse>> GetMainCategory(FBGetMainCategoryRequest request);
         public Task<List<FBGetProductListByMainCategoryResponse>> GetProductListByMainCategory(FBGetProductListByMainCategoryRequest request);
         public Task<List<FBGetLatestProductListResponse>> GetLatestProductList(FBGetLatestProductListRequest request);
         public Task<List<FBGetPopularProductListResponse>> GetPopularProductList(FBGetPopularProductListRequest request);
         public Task<List<FBGetPromotionProductListResponse>> GetPromotionProductList(FBGetPromotionProductListRequest request);
    }
}