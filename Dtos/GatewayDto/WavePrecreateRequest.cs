using System.Collections.Generic;

namespace MayMayShop.API.Dtos.GatewayDto
{
    public class WavePrecreateRequest
    {
        public int time_to_live_in_seconds {get;set;}
        public string merchant_id {get;set;}
        public string order_id {get;set;}
        public string merchant_reference_id {get;set;}
        public string frontend_result_url {get;set;}
        public string backend_result_url {get;set;}
        public int amount {get;set;}
        public string payment_description {get;set;}
        public string merchant_name {get;set;}
        public string items {get;set;}
        public string hash {get;set;}
    }
}