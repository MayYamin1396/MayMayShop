namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetDeliverySlotRequest
    {
        public int FromEstDeliveryDay { get; set; }

        public int ToEstDeliveryDay { get; set; }
    }
}