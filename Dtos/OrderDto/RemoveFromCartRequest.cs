namespace MayMayShop.API.Dtos.OrderDto
{
    public class RemoveFromCartRequest
    {
        public int ProductId { get; set; }

        public int SkuId { get; set; }
    }
}