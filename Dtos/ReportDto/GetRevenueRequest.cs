using System;

namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetRevenueRequest
    {
        public DateTime FromDate {get;set;}
        public DateTime ToDate {get;set;}
    }
}