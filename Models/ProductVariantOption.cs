namespace MayMayShop.API.Models
{
    public class ProductVariantOption
    {
        public int ProductId { get; set; }

        public int VariantId { get; set; }

        public int ValueId { get; set; }

        public string ValueName { get; set; }
    }
}