using System;

namespace MayMayShop.API.Models
{
    public partial class ProductPrice
    {
        public int Id { get; set; }

        public double Price { get; set; }

        public bool isActive { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
        
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
