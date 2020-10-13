namespace MayMayShop.API.Dtos.ReportDto
{
    public class GetSalesAndPerformanceResponse
    {
        public int TotalTraffic {get;set;}
        public double TrafficRate {get;set;}
        public int NewUser {get;set;}
        public double NewUserRate {get;set;}
        public int Sales {get;set;}
        public double SaleRate {get;set;}
        public int Performance {get;set;}
        public double PerformanceRate {get;set;}
    }
}