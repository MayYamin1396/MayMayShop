using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class UpdateOrderStatusRequest
    {
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
    }
}
