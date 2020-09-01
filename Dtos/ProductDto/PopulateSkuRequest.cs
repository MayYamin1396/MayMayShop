using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class PopulateSkuRequest
    {
        public int ProductCategoryId { get; set; }

        public List<Options> Options { get; set; }
    }

    public class Options
    {
        public int VariantId { get; set; }
        public List<string> OptionValue  { get; set; }
    }

    
}