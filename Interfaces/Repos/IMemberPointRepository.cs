using System.Collections.Generic;
using System.Threading.Tasks;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.MembershipDto;
using MayMayShop.API.Dtos.OrderDto;

namespace MayMayShop.API.Interfaces.Repos
{
    public interface IMemberPointRepository
    {
        Task<List<GetConfigMemberPointResponse>> GetConfigMemberPoint(string token);
        Task<GetConfigMemberPointResponse> GetConfigMemberPointById(int id, string token);        
        Task<ResponseStatus> ReceivedMemberPoint(ReceivedMemberPointRequest request,string token);
        Task<ResponseStatus> CreateProductReward(CreateProductRewardRequest request);
        Task<ResponseStatus> UpdateProductReward(UpdateProductRewardRequest request);
        Task<List<GetRewardProductResponse>> GetRewardProduct(GetRewardProductRequest request);
        Task<GetRewardProductByIdResponse> GetRewardProductById(GetRewardProductByIdRequest request);        
        Task<GetRewardProductDetailResponse> GetRewardProductDetail(GetRewardProductDetailRequest request,int currentUserLogin,string token);
        Task<PostOrderResponse> RedeemOrder(RedeemOrderRequest request,int currentUserLogin,string token);
        Task<GetCartDetailForRewardResponse> GetCartDetailForReward(int productId,int skuId,int currentUserLogin,string token);
        Task<PostOrderByKBZPayResponse> RedeemOrderByKBZPay(RedeemOrderRequest request,int currentUserLogin,string token);
        Task<List<GetConfigMemberPointProductCategory>> GetProductCategoryForCreateConfigMemberPoint(string token);
    }
}
