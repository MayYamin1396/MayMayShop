namespace MayMayShop.Dtos.MiscellaneousDto
{
    public class GetPaymentServiceForSellerResponse
    {
        public int PaymentServiceId{get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public bool IsActive{get;set;}
        public string PaymentType {get;set;}
        public string BackgroundUrl {get;set;}
        public string HolderName {get;set;}
        public string AccountNo {get;set;}
        public bool? IsPaymentGateWay {get;set;}
    }
}