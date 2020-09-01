using System.Collections.Generic;

namespace MayMayShop.API.Dtos.MembershipDto
{
     public class GetCartDetailForRewardResponse : ResponseStatus
    {
        public double TotalAmt { get; set; }
        public double DeliveryFee { get; set; }
        public double NetAmt { get; set; }
        public int TotalPoint { get; set; }
        public int  MyOwnPoint { get; set; }
        public GetCartDetailForRewardProductInfo ProductInfo { get; set; }

        public GetCartDetailForRewardDeliveryInfo DeliveryInfo { get; set; }

        public List<GetCartDetailForRewardPaymentService> PaymentService { get; set; }
    }

    public class GetCartDetailForRewardProductInfo
    {
        public int ProductId { get; set; }
        public string ProductUrl { get; set; }
        public int SkuId { get; set; }
        public string Name { get; set; }
        public double OriginalPrice { get; set; }
        public double RewardAmount { get; set; }
        public int RewardPercent { get; set; }
        public double FixedPrice { get; set; }
        public int Point { get; set; }
        public string Variation { get; set; }
        public int Qty { get; set; }
        public int AvailableQty { get; set; }
    }

    public class GetCartDetailForRewardDeliveryInfo
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

    public class GetCartDetailForRewardPaymentService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
    }
}