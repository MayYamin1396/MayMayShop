using System;
using System.Collections.Generic;
using MayMayShop.API.Dtos.MiscellanceousDto;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetOrderDetailResponse
    {
        public int OrderId {get;set;}
        public string VoucherNo { get; set; }

        public DateTime OrderDate { get; set; }

        public double TotalAmt { get; set; }
        public double DeliveryFee { get; set; }
        public double NetAmt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? OrderCancelDate { get; set; }

        public string OrderCancelBy { get; set; }

        public int UserId { get; set; }

        public string UserUrl { get; set; }

        public virtual GetOrderDetailOrderStatus OrderStatus { get; set;}

        public virtual List<GetOrderDetailOrderItem> OrderItem { get; set; }

        public virtual GetOrderDetailDeliveryInfo DeliveryInfo { get; set; }

        public virtual List<GetOrderDeailOrderPaymentInfo> PaymentInfo { get; set; }
        public List<GetCartDetailPaymentService> PaymentService { get; set; }
        public List<GetPaymentServiceForBuyerResponse> NewPaymentService { get; set; }

        // public int OrderCount { get; set; }
        
        // public List<OrderIdList> OrderIdList { get ; set; }

    }

    public class OrderIdList
    {
        public int OrderId { get; set; }
    }

    public class GetOrderDetailOrderStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }

    }

    public class GetOrderDetailOrderItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double OriginalPrice { get; set; }

        public double PromotePrice { get; set; }
        public int? PromotePercent {get;set;}

        public int SkuId { get; set; }

        public int Qty { get; set; }

        public string SkuValue { get; set; }
        
        public string Url { get; set; }

    }

    public class GetOrderDetailDeliveryInfo
    {
        public string Name { get; set; }     

        public string CityName { get; set; }

        public string TownshipName { get; set; }

        public int? CityId { get; set; }        

        public int? TownshipId { get; set; }

        public string Address { get; set; }

        public string PhNo { get; set; }

        public string Remark { get; set; } 
        public string DeliveryDate { get; set; }      

        public int? DeliveryServiceId { get; set; }

        public virtual GetOrderDeailDeliveryService DeliveryService { get; set; }
    }


    public class GetOrderDeailDeliveryService
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int FromEstDeliveryDay { get; set; }

        public int ToEstDeliveryDay { get; set; }

        public double ServiceAmount { get; set; }

        public string ImgPath { get; set; }
    }

    public class GetOrderDeailOrderPaymentInfo
    {
        public int Id { get; set; }

        public DateTime TransactionDate { get; set; }

        // public string[] ApproveImg { get; set; }
        public string ApproveImg { get; set; }

        public string PhoneNo { get; set; }

        public string Remark { get; set; }
        public string SellerRemark { get; set; }
        public int? BankId {get;set;}       
        public string BankName {get;set;}
        public string BankLogo {get;set;}
        public bool IsApproved { get; set; }              
        public virtual GetOrderDeailPaymentService PaymentService { get; set; }

        public virtual GetOrderDetailPaymentStatus PaymentStatus { get; set; }
    }

    public class GetOrderDeailPaymentService
    {
        public string Name { get; set; }
        public string BankName {get;set;}
        public string ImgPath { get; set; }
    }

    public class GetOrderDetailPaymentStatus
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
    
}