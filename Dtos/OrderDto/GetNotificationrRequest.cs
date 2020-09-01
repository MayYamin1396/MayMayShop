using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Dtos.OrderDto
{
    public class GetNotificationRequest
    {
        public DateTime? Date { get; set; }
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