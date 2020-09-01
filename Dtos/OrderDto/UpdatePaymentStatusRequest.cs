using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class UpdatePaymentStatusRequest
    {
        public int PaymentInfoId { get; set; }
        public int PaymentStatusId { get; set; }
        public string SellerRemark {get;set;}
    }
}
