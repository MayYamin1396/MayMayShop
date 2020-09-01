namespace MayMayShop.API.Dtos.ProductDto
{
    public class GetRevenueResponse
    {
        public double Revenue_Android {get;set;}
        public int SoldItem_Android {get;set;}
        public int TotalVisitor_Android {get;set;}
        public double Revenue_IOS {get;set;}
        public int SoldItem_IOS {get;set;}
        public int TotalVisitor_IOS {get;set;}
        public double Revenue_Web {get;set;}
        public int SoldItem_Web {get;set;}
        public int TotalVisitor_Web {get;set;}
    }
}