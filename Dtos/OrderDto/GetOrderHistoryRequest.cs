using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetOrderHistoryRequest
    {
        [Required]
        public int UserId{get;set;}
        public string VoucherNo{ get; set; }
        public int PaymentStatusId{get;set;}
        public int OrderStatusId{get;set;}        
        public string OrderDate{get;set;}
        public string PaymentDate{get;set;}
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
    }
}