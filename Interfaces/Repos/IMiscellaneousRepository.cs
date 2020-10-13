using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.MiscellanceousDto;
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
        Task<CreateVariantResponse> CreateVariant(CreateVariantRequest request,int currentUserLogin);
        Task<ResponseStatus> UpdateVariant(UpdateVariantRequest request,int currentUserLogin);
        Task<ResponseStatus> DeleteVariant(int variantId,int currentUserLogin);
        Task<List<GetPolicyResponse>> GetPolicy();
        Task<GetPolicyResponse> GetPolicyById(int id);
        Task<ResponseStatus> DeletePolicy(int Id);
        Task<ResponseStatus> CreatePolicy(CreatePolicyRequest request, int currentUserLogin);
        Task<ResponseStatus> UpdatePolicy(UpdatePolicyRequest request, int currentUserLogin);
        Task<ResponseStatus> CreateBanner(CreateBannerRequest request,int currentUserLogin,string Url);
        Task<ResponseStatus> UpdateBanner(UpdateBannerRequest request,int currentUserLogin,ImageUrlResponse image);
        Task<ResponseStatus> DeleteBanner(int id, int currentUserLogin);
        Task<GetBannerResponse> GetBannerById(int id);
        Task<List<GetBannerResponse>> GetBannerList(int bannerType);
        Task<List<GetBannerLinkResponse>> GetBannerLink();
        
        #region Activity Log API
        Task<string> GetLastActiveByUserId(int id);
        #endregion

        #region Payment service
        Task<List<GetPaymentServiceForSellerResponse>> GetPaymentServiceForSeller();
        Task<GetPaymentServiceForSellerResponse> GetPaymentServiceDetail(int paymentServiceId);
        Task<ResponseStatus> UpdatePaymentService(UpdatePaymentServiceRequest request);

        Task<List<GetBankResponse>> GetBankListForSeller();
        Task<GetBankResponse> GetBankDetail(int id);
        Task<ResponseStatus> UpdateBank(UpdateBankRequest request);

        #endregion

        #region  Category Icon
        Task<ResponseStatus> CreateCategoryIcon(CreateCategoryIconRequest request,List<ImageUrlResponse> imgList);
        #endregion

        Task<List<GetPaymentServiceForBuyerResponse>> GetPaymentServiceForBuyer();  

        Task<ResponseStatus> AddBrand(AddBrandRequest request, string imageurl, string logourl);
        Task<ResponseStatus> UpdateBrand(UpdateBrandRequest request, string imageurl, string logourl);
        Task<ResponseStatus> DeleteBrand(int id);
        Task<List<GetBrandResponse>> GetBrand();
    }
}
