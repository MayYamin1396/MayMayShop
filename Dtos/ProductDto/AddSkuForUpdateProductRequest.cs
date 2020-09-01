using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
     public class AddSkuForUpdateProductRequest
    {
        public int ProductId  { get; set; }
        public List<VariantList> VariantList { get; set; }        
    }
    public class VariantList
    {
        public int VariantId { get; set; }
        public string VariantName { get; set; }
    }
}