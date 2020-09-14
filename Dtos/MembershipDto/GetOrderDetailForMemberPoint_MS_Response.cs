using System;

namespace MayMayShop.API.Dtos.MembershipDto
{
    public class GetOrderDetailForMemberPoint_MS_Response
    {
        public int OrderId {get;set;}
        public string VoucherNo {get;set;}
        public DateTime OrderDate {get;set;} 
        public string ProductUrl {get;set;}
        public int TotalItem {get;set;}
        public string OrderStatus {get;set;}
        public string PaymentStatus {get;set;}
        public double TotalAmount {get;set;}
        public double DeliveryFee {get;set;}
        public double NetAmount {get;set;}
        
    }
}