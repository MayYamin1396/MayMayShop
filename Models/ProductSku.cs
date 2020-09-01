using System;

namespace MayMayShop.API.Models
{
    public class ProductSku
    {        
        public int ProductId { get; set; }

        public int SkuId { get; set; }

        public string Sku { get; set; }

        public int Qty { get; set; }

        public double Price { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}