using System.ComponentModel.DataAnnotations;
namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetProductDetailRequest
    {
        [Required]
        public int ProductId { get; set; }
    }
}