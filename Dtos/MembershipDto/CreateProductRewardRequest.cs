using System;

namespace MayMayShop.API.Dtos.MembershipDto
{
    public class CreateProductRewardRequest
    {
        public int ProductId {get;set;}
        public int Point {get;set;}
        public int RewardPercent {get;set;}
        public double FixedAmount {get;set;}
        public DateTime StartDate {get;set;}
        public DateTime EndDate {get;set;}
    }
}