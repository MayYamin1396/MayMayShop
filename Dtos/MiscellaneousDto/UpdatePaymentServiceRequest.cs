namespace MayMayShop.API.Dtos.MiscellaneousDto
{
    public class UpdatePaymentServiceRequest
    {
        public int PaymentServiceId{get;set;}
        public string Name {get;set;}
        public string HolderName {get;set;}
        public string AccountNo {get;set;}
        public bool IsActive {get;set;}
    }
}