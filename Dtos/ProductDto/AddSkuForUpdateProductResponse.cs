namespace MayMayShop.API.Dtos.ProductDto
{
    public class AddSkuForUpdateProductResponse : ResponseStatus
    {
        public int ProductId { get; set; }

        public int SkuId { get; set; }

        public string VariantOptions { get; set; }
    }
}