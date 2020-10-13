namespace MayMayShop.API.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int SkuId { get; set; }

        public int Qty { get; set; }

        public double Price { get; set; }
        public double OriginalPrice { get; set; }
        public double Discount{ get;set;}
        public int? PromotePercent{get;set;}
        public double? PromoteFixedPrice{get;set;}
        public int? Point {get;set;}
        public double? FixedAmount {get;set;}
        public int? RewardPercent {get;set;}
    }
}