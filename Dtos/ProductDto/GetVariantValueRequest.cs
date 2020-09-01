namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetVariantValueRequest
    {
        public int ProductId  { get; set; }
        public int SelectedVariantId { get; set; }
        public int SelectedValueId { get; set; }
        public int CurrentVariantId { get; set; }
    }
}