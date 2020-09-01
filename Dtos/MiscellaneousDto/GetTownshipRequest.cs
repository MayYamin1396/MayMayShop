using System.ComponentModel.DataAnnotations;

namespace MayMayShop.Dtos.MiscellaneousDto
{
    public class GetTownshipRequest
    {
        [Required]
        public int CityId { get; set; }
    }
}