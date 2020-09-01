using System;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class PopulateSkuResponse : ResponseStatus
    {
        public Guid ProductId { get; set; }

        public int SkuId { get; set; }

        public string VariantOptions { get; set; }
    }
}