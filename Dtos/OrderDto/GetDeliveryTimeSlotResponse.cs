using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetDeliverySlotResponse
    {
        public WeekOne WeekOne{get;set;}
        public WeekTwo WeekTwo{get;set;}    
        public string SelectedDate { get; set; }    
        public string UserSelectedDeliveryDate { get; set; }      
        public List<Occupied> OccupiedList{get;set;}
        public List<DeliverySerivceTime> DeliverySerivceTimeList{get;set;}
        
    }
    public class WeekOne
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate  { get; set; }
    }
    public class WeekTwo
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate  { get; set; }
    }
    public class Occupied
    {
        public DateTime DeliveryDate { get; set; }
        public string FromTime { get; set; }
        public string ToTime  { get; set; }
    }
    public class DeliverySerivceTime
    {
        public string FromTime { get; set; }
        public string ToTime  { get; set; }
    }
}