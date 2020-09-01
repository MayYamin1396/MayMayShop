using System;

namespace MayMayShop.API.Models
{
    public class ProductReward
    {
        public int Id {get;set;}
        public int ProductId {get;set;}
        public virtual Product Product { get; set; }
        public int Point {get;set;}
        public double FixedAmount {get;set;}
        public int RewardPercent {get;set;}
        public double RewardAmount {get;set;}
        public DateTime StartDate {get;set;}
        public DateTime EndDate {get;set;}
    }
}