using System;
using System.Collections.Generic;
using MayMayShop.API.Dtos.MiscellanceousDto;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetCartDetailResponse : ResponseStatus
    {
        public double TotalAmt { get; set; }
        public double DeliveryFee { get; set; }
        public double NetAmt { get; set; }

        public List<GetCartDetailProductInfo> ProductInfo { get; set; }

        public GetCartDetailDeliveryInfo DeliveryInfo { get; set; }

        public List<GetCartDetailPaymentService> PaymentService { get; set; }
        public List<GetPaymentServiceForBuyerResponse> NewPaymentService { get; set; }
        public List<ProductIssues> ProductIssues{get;set;}
    }

    public class GetCartDetailProductInfo
    {
        public int ProductId { get; set; }
        public string ProductUrl { get; set; }

        public int SkuId { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public double PromotePrice { get; set; }

        public string Variation { get; set; }

        public int Qty { get; set; }

        public int AvailableQty { get; set; }
    }

    public class GetCartDetailDeliveryInfo
    {
        public int DeliveryServiceId { get; set; }

        public int CityId { get; set; }

        public int TownshipId { get; set; }

        public string CityName { get; set; }

        public string TownshipName { get; set; }

        public string AreaInfo { get; set; }

        public int FromEstDeliveryDay { get; set; }

        public int ToEstDeliveryDay { get; set; }

        public double DeliveryAmt { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoNo { get; set; }

        public string Remark { get; set; }

        public string DeliveryDate { get; set; }

        public string DeliveryFromTime { get; set; }

        public string DeliveryToTime { get; set; }

    }

    public class GetCartDetailPaymentService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
    }
}