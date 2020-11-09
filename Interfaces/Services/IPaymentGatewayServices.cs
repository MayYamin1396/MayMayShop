
using System.Collections.Generic;
using System.Threading.Tasks;
using MayMayShop.API.Dtos.GatewayDto;
using MayMayShop.API.Dtos.OrderDto;

namespace MayMayShop.API.Interfaces.Services
{
    public interface IPaymentGatewayServices
    {
        Task<PostOrderByKBZPayResponse> KBZPrecreate(string orderId, double totalAmt,int platform);
        Task<KBZPQueryOrderResponse> KBZQueryOrder(string TransactionId);

        Task<PostOrderByWavePayResponse> WavePayPrecreate(string TransactionId,double NetAmount, List<ProductItem> Items, string payment_description,int platform);

        string GenerateSHA256Hash_WaveTransaction (CheckWaveTransactionStatusRequest request);
    }
}