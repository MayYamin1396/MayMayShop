using System;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetOrderHistoryResponse
    {
         public int OrderId { get; set; }
        public string VoucherNo { get; set; }
        public string ProductUrl { get; set; }        
        public double Price { get; set; }
        public int Qty { get; set; }        
        public int OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public int PaymentStatusId { get; set; }
        public string PaymentStatusName { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PaymentServiceImgPath { get; set; }
    }
}