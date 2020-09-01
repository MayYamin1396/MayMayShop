using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetProductSkuResponse
    {
         public Guid ProductId { get; set; }

        public int SkuId { get; set; }

        public string VariantOptions { get; set; }

        // public string VariantName { get ; set; }

        // public int VariantId { get; set; }

        public int Qty { get; set; }
        
    }
}