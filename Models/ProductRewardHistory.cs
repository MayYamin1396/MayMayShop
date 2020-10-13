using System;

namespace MayMayShop.API.Models
{
    public class ProductRewardHistory
    {
        public int Id {get;set;}
        public int ProductId {get;set;}
        public DateTime StartDate {get;set;}
        public DateTime EndDate {get;set;}
    }
}