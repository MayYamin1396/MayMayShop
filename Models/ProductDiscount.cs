using System;

namespace MayMayShop.API.Models
{
    public class ProductDiscount
    {
        public int Id { get; set; }

        public string DiscountName {get;set;}

        public int ProductId { get; set; }

        public int DiscountPercentage { get; set; }

        public DateTime? StartDate{get;set;}
        
        public DateTime? EndDate { get; set; }
    }
}