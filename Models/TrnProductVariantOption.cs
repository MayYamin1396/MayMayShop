using System;

namespace MayMayShop.API.Models
{
    public class TrnProductVariantOption
    {
        public Guid ProductId { get; set; }

        public int VariantId { get; set; }

        public int ValueId { get; set; }

        public string ValueName { get; set; }
    }
}