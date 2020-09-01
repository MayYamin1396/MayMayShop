using System.Collections.Generic;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetProductVariantResponse
    {
        public List<ValueList> ValueList1 { get; set; }
        public List<ValueList> ValueList2 { get; set; }
        public List<ValueList> ValueList3 { get; set; }
        
    }
    public class ValueList
    {
        public string ValueName { get; set; }
        public int ValueId { get; set; }
        public int VariantId { get; set; }
    }
}