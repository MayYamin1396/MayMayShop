using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class RedeemOrderRequest
    {
        public double TotalAmt { get; set; }
        public double NetAmt { get; set; }
        public double DeliveryFee { get; set; }
        public int TotalPoint {get;set;}
        public int MyOwnPoint {get;set;}
        public int PlatForm {get;set;}
        public List<RedeemOrderProductInfo> ProductInfo { get; set; }
        public RedeemOrderDeliveryInfo DeliveryInfo { get; set; }        
        public RedeemOrderPaymentService PaymentInfo { get; set; }
    }

    public class RedeemOrderProductInfo
    {
        public int ProductId { get; set; }
        public int SkuId { get; set; }
        public int Qty { get; set; }
        public double RewardAmount {get;set;}
        public int Point {get;set;}
        public int RewardPercent {get;set;}
    }

    public class RedeemOrderDeliveryInfo
    {
        public string Name { get; set; }        
        public int CityId { get; set; }
        public int TownshipId { get; set; }
        public string PhoNo { get; set; }
        public string Address { get; set; }
        public string Remark { get; set; }
        public int DeliverServiceId { get; set; }           
        public DateTime DeliveryDate{get;set;}
        public string FromTime { get; set; }
        public string ToTime { get; set; } 
    }

    public class RedeemOrderPaymentService
    {
        public int PaymentServiceId { get; set; }
        public int BankId {get;set;}
        public string PhoNo { get; set; }
        public string Remark { get; set; }
        public RedeemOrderPaymentImgage ApprovalImage { get; set; }
    }
    public class RedeemOrderPaymentImgage
    {
        public string ApprovalImage { get; set;}
        public string ApprovalImageExtension { get; set; }
    }
}