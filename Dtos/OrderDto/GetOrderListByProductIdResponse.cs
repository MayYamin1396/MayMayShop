using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetOrderListByProductIdResponse
    {
        public string ProductName { get; set; }
        public string ProductUrl { get; set; }
        public int ProductId { get; set; }
        public int OrderCount { get; set; }
        public double Price { get; set; }
        public int TotalQty { get; set; }
        public string SkuValue { get; set; }
        public List<UserResponse> UserList { get; set; }

    }
    public class UserResponse
    {
        public int OrderId { get; set; }
        public int Qty { get; set; }
        public string Sku { get; set; }
        public string OrderStatus { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentInfoId { get; set; }
        public string PaymentStatus { get; set; }
        public int PaymentStatusId { get; set; }
        public string PaymentServiceName { get; set; }
        public string PaymentServiceImgPath { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserUrl { get; set; }
        public DateTime OrderCreatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
