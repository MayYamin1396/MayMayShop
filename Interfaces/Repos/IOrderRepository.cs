

using System.Collections.Generic;
using System.Threading.Tasks;
using MayMayShop.API.Dtos.OrderDto;
using MayMayShop.API.Dtos;


namespace MayMayShop.API.Interfaces.Repos
{
    public interface IOrderRepository
    {
        Task<ResponseStatus> AddToCart(AddToCartRequest request, int userId,int platform);
        Task<RemoveFromCartResponse> RemoveFromCart(RemoveFromCartRequest request, int userId,int platform);
        Task<GetCartDetailResponse> GetCartDetail(int userId, string token);
        Task <ResponseStatus> UpdateDeliveryInfo(UpdateDeliveryInfoRequest request, int userId,string token);
        Task <ResponseStatus> UpdateDeliveryDateAndTime(UpdateDeliveryDateAndTimeRequest request, int userId,string token);
        Task<GetDeliverySlotResponse> GetDeliverySlot(GetDeliverySlotRequest request,int userId);
        Task<PostOrderResponse> PostOrder(PostOrderRequest req, int userId,string token,int platform);
        Task<ResponseStatus> PostOrderActivity(int orderId, int userId,string token,int platform);
        Task<PostOrderByKBZPayResponse> PostOrderByKBZPay(PostOrderRequest req,int userId, string token);
        Task<PostOrderByWavePayResponse> PostOrderByWavePay(PostOrderRequest req,int userId, string token);
        Task<PostOrderResponse> CheckKPayStatus(string transactionId,int userId,string token,int platform);

        Task<> CheckWaveTransactionStatus (CheckWaveTransactionStatusRequest request);
        Task <ResponseStatus> UpdateProductCart(UpdateProductCartRequest request, int userId);
        Task<List<GetOrderHistoryResponse>> GetOrderHistory(GetOrderHistoryRequest request);
         Task<List<GetOrderHistoryResponse>> GetOrderHistorySeller(GetOrderHistorySellerRequest request);
         Task<List<GetNotificationResponse>> GetNotificationBuyer(GetNotificationRequest request,int userId,string token);
         Task<List<GetNotificationResponse>> GetNotificationSeller(GetNotificationRequest request,int userId,string token);
         Task<ResponseStatus> SeenNotification(SeenNotificationRequest request,int userId);
        Task<List<GetOrderListByProductResponse>> GetOrderListByProduct(GetOrderListByProductRequest request);
        Task<GetOrderListByProductIdResponse> GetOrderListByProductId(GetOrderListByProductIdRequest request);
        Task<ResponseStatus> UpdateOrderStatus(UpdateOrderStatusRequest request, int currentUserLogin,int platform);
        Task<ResponseStatus> UpdatePaymentStatus(UpdatePaymentStatusRequest request, int currentUserLogin,string token,int platform);
        Task<ResponseStatus> UpdateDeliveryServiceStatus(UpdateDeliveryServiceStatusRequest request, int currentUserLogin,string token);
        Task<ResponseStatus> SellerOrderCancel(OrderCancelRequest request, int currentUserLogin,int platform);
        Task<ResponseStatus> BuyerOrderCancel(OrderCancelRequest request, int currentUserLogin,string token,int platform);
        Task<ResponseStatus> ChangeDeliveryAddress(ChangeDeliveryAddressRequest request);
        Task<ResponseStatus> PaymentAgain(PaymentAgainRequest request, int currentUserLogin,int platform,string token);
        Task<ResponseStatus> PaymentApprove(PaymentApproveRequest request, int currentUserLogin,int platform);        
        Task<GetOrderDetailResponse> GetOrderDetail(int orderId,string token);
        Task<List<string>> GetVoucherNoSuggestion(GetVoucherNoSuggestionRequest request);       
        Task<List<string>> GetVoucherNoSuggestionSeller(GetVoucherNoSuggestionSellerRequest request);    
        Task<GetVoucherResponse> GetVoucher(int OrderId,string token);  
        Task<GetPOSVoucherResponse> GetPOSVoucher(int OrderId,int userId,string token);
        Task<bool> CallBackKPayNotify(string transactionId);         
    }
}