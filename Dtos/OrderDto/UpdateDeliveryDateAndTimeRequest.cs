using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class UpdateDeliveryDateAndTimeRequest
    {
        public DateTime DeliveryDate { get; set; }
        public string DeliveryFromTime { get; set; }
        public string DeliveryToTime { get; set; }
        public List<ProductCart> ProductCarts { get; set; }
    }    
}