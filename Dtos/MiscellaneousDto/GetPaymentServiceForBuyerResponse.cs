using System.Collections.Generic;

namespace MayMayShop.API.Dtos.MiscellanceousDto
{
    public class GetPaymentServiceForBuyerResponse
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string PaymentType {get;set;}
        public string Url {get;set;}
        public int SerNo {get;set;}
        public List<PaymentServiceGateWay> PaymentServiceGateWay{get;set;}
    }
    public class PaymentServiceGateWay
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public string Url {get;set;}
        public bool IsPaymentGateWay {get;set;}
    }
}