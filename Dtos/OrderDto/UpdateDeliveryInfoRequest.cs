using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class UpdateDeliveryInfoRequest
    {
        public int UserId { get; set; }        
        public string Address { get; set; }
        public int TownshipId { get; set; }
        public int CityId { get; set; }
        public string Remark { get; set; }
        public List<ProductCart> ProductCarts { get; set; }
    }  

}