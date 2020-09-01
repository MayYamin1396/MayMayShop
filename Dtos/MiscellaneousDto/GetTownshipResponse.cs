using System.Collections.Generic;
using MayMayShop.API.Dtos;

namespace MayMayShop.Dtos.MiscellaneousDto
{
    public class GetTownResponse
    {
        public List<GetTownResponseArry> TownList { get; set; }
    }
    public class GetTownResponseArry
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CityId { get; set; }
        public int DeliveryServiceId { get; set; }        
        public int FromEstDeliveryDay { get; set; }
        public int ToEstDeliveryDay { get; set; }
        public double ServiceAmount { get; set; }
        public int DeliveryServiceTimeId { get; set; }
        public string DeliveryTimeGap { get; set; }
        //public List<Township> TownList { get; set; }
    }
}