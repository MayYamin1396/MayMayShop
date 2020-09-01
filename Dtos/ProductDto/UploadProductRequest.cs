using Microsoft.AspNetCore.Http;
namespace MayMayShop.API.Dtos.ProductDto
{
    public class UploadProductRequest
    {
        public IFormFile File { get; set; }
    }
}