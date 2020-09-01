using System;

namespace MayMayShop.API.Models
{
    public partial class ProductPromotion
    {
        public int Id { get; set; }

        public int Percent { get; set; }

        public double FixedAmt { get; set; }

        public double TotalAmt { get; set; }

        public string PromoCode { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}