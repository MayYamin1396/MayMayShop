namespace MayMayShop.API.Dtos.GatewayDto
{
   public class KBZNotifyRequest
    {
        public Request Request{get;set;}
    }
    public class Request{
        public int notify_time {get;set;}
        public string merch_code {get;set;}
        public string merch_order_id {get;set;}
        public string mm_order_id {get;set;}
        public string trans_currency {get;set;}
        public string total_amount {get;set;}
        public string trade_status {get;set;}
        public int trans_end_time {get;set;}
        public string callback_info {get;set;}
        public string nonce_str {get;set;}
        public string sign_type {get;set;}
        public string appid {get;set;}
        public string sign {get;set;}

    }
}