namespace MayMayShop.API.Dtos.OrderDto
{
    public class AddToCartRequest
    {
        public int ProductId { get; set; }

        public int SkuId { get; set; }

        public int Qty { get; set; }
    }
}