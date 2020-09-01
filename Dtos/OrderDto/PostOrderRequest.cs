using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class PostOrderRequest
    {
        public double TotalAmt { get; set; }
        public double NetAmt { get; set; }
        public double DeliveryFee { get; set; }
        public int Platform {get;set;}
        public List<PostOrderProductInfo> ProductInfo { get; set; }

        public PostOrderDeliveryInfo DeliveryInfo { get; set; }
        
        public PostOrderPaymentService PaymentInfo { get; set; }
    }

    public class PostOrderProductInfo
    {
        public int ProductId { get; set; }

        public int SkuId { get; set; }

        public double Price { get; set; }

        public int Qty { get; set; }
    }

    public class PostOrderDeliveryInfo
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

    public class PostOrderPaymentService
    {
        public int PaymentServiceId { get; set; }
        public int BankId {get;set;}

        public string PhoNo { get; set; }

        public string Remark { get; set; }
        public PostOrderPaymentImgage ApprovalImage { get; set; }
    }

    public class PostOrderPaymentImgage
    {
        public string ApprovalImage { get; set;}

        public string ApprovalImageExtension { get; set; }
    }
}