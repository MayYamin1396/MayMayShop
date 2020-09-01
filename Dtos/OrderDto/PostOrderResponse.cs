using MayMayShop.API.Dtos.GatewayDto;

namespace MayMayShop.API.Dtos.OrderDto
{
   public class PostOrderResponse : ResponseStatus
    {
        public int OrderId { get; set; }

        public int? Timestamp { get; set; }

        public string NonceStr { get; set; }

        public KBZPrecreateResponse Precreate { get; set; }
    }
}