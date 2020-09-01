
using System.Threading.Tasks;
using MayMayShop.API.Dtos.GatewayDto;
using MayMayShop.API.Dtos.OrderDto;

namespace MayMayShop.API.Interfaces.Services
{
    public interface IPaymentGatewayServices
    {
        Task<PostOrderByKBZPayResponse> KBZPrecreate(string orderId, double totalAmt,int platform);
        Task<KBZPQueryOrderResponse> KBZQueryOrder(string TransactionId);
    }
}