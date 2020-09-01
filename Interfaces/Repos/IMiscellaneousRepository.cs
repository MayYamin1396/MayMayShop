using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.MiscellaneousDto;
using MayMayShop.Dtos.MiscellaneousDto;

namespace MayMayShop.API.Interfaces.Repos
{
    public interface IMiscellaneousRepository
    {
        Image FixedSize(Image imgPhoto, int width, int height);
        Task<List<GetMainCategoryResponse>> GetMainCategory();
        Task<List<GetSubCategoryResponse>> GetSubCategory(GetSubCategoryRequest request);
        Task<List<SearchTagResponse>> SearchTag(SearchTagRequest request);
        Task<List<GetBankResponse>> GetBank();
        Task<List<GetTagResponse>> GetTag();
        Task<List<SearchCategoryResponse>> SearchCategory(string searchText);
        Task<List<GetCategoryIconResponse>> GetCategoryIcon();
        Task<ResponseStatus> CreateMainCategory(CreateMainCategoryRequest request,int currentUserLogin);
        Task<ResponseStatus> UpdateMainCategory(UpdateMainCategoryRequest request,int currentUserLogin);
        Task<ResponseStatus> DeleteMainCategory(int productCategoryId,int currentUserLogin);
        Task<GetMainCategoryByIdResponse> GetMainCategoryById(int productCategoryId);
        Task<GetSubCategoryResponse> CreateSubCategory(CreateSubCategoryRequest request,int currentUserLogin);
        Task<ResponseStatus> UpdateSubCategory(UpdateSubCategoryRequest request,int currentUserLogin);
        Task<ResponseStatus> DeleteSubCategory(int productCategoryId,int currentUserLogin);
        Task<GetSubCategoryByIdResponse> GetSubCategoryById(int productCategoryId);
        Task<ResponseStatus> CreateVariant(CreateVariantRequest request,int currentUserLogin);
        Task<ResponseStatus> UpdateVariant(UpdateVariantRequest request,int currentUserLogin);
        Task<ResponseStatus> DeleteVariant(int variantId,int currentUserLogin);
        Task<List<GetPolicyResponse>> GetPolicy();
        Task<ResponseStatus> CreateBanner(CreateBannerRequest request,int currentUserLogin,string Url);
        Task<ResponseStatus> UpdateBanner(UpdateBannerRequest request,int currentUserLogin,ImageUrlResponse image);
        Task<ResponseStatus> DeleteBanner(int id, int currentUserLogin);
        Task<GetBannerResponse> GetBannerById(int id);
        Task<List<GetBannerResponse>> GetBannerList(int bannerType);
        Task<List<GetBannerLinkResponse>> GetBannerLink();
        
    }
}
