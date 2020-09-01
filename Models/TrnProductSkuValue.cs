using System;

namespace MayMayShop.API.Models
{
    public class TrnProductSkuValue
    {
        public Guid ProductId { get; set; }

        public int  SkuId { get; set; }

        public int VariantId { get; set; }

        public int ValueId { get; set; }
    }
}