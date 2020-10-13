using System;

namespace MayMayShop.API.Dtos.ReportDto
{
    public class GetSalesAndPerformanceRequest
    {
        public DateTime FromDate {get;set;}
        public DateTime ToDate {get;set;}
    }
}