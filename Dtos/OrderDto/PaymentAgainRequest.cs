using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class PaymentAgainRequest
    {
        public int OrderId { get; set; }
        public int PaymentServiceId { get; set; }
        public string PhoNo { get; set; }
        public string Remark { get; set; }
        public int BankId {get;set;}
        public PostOrderPaymentImgage ApprovalImage { get; set; }
    }
}
