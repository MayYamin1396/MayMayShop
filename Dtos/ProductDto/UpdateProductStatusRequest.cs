namespace MayMayShop.API.Dtos.ProductDto
{
    public class UpdateProductStatusRequest
    {
        public int ProductId {get;set;}
        public string ProductStatus {get;set;}
    }
}