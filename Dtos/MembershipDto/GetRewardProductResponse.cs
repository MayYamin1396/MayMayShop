using System;

namespace MayMayShop.API.Dtos.MembershipDto
{
    public class GetRewardProductResponse : ResponseStatus
    {
        public int Id {get;set;}
        public int ProductId {get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public DateTime StartDate {get;set;}
        public DateTime EndDate {get;set;}
        public double OriginalPrice {get;set;}
        public int Point {get;set;}
        public double FixedAmount {get;set;}
        public double RewardAmount {get;set;}
        public int RewardPercent {get;set;}
    }
}