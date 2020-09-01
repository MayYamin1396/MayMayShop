namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetDeliveryServiceRateResponse
    {
        public int DeliveryServiceId { get; set; }

        public int CityId { get; set; }

        public int TownshipId { get; set; }
        
        public int FromEstDeliveryDay { get; set; }

        public int ToEstDeliveryDay { get; set; }

        public double ServiceAmount { get; set; }   
             
    }
}