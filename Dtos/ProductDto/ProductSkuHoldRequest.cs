using System;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class ProductSkuHoldRequest
    {
        public Guid ProductId { get; set; }
        public List<Sku> Sku { get; set; }
    }
}