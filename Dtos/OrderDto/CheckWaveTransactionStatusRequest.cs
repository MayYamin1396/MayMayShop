namespace MayMayShop.API.Dtos.OrderDto
{
    public class CheckWaveTransactionStatusRequest
    {
        public string status{get;set;}
        public string merchantId {get;set;}
        public string orderId {get;set;}

        public string merchantReferenceId {get;set;}
        public string frontendResultUrl {get;set;}
        public string backendResultUrl {get; set;}
        public string initiatorMsisdn {get;set;}
        public string amount {get;set;}
        public string timeToLiveSeconds {get;set;}

        public string paymentDescription {get;set;}
        public string currency {get;set;}
        public string hashValue {get;set;}
        public string transactionId {get;set;}

        public string paymentRequestId {get;set;}
        public string requestTime {get;set;}
    }
}