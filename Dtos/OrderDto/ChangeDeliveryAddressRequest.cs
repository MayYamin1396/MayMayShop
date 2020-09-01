using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class ChangeDeliveryAddressRequest
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        public int TownshipId { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        public string Address { get; set; }
        public string Remark { get; set; }
        public string PhoneNumber { get; set; }
    }
}
