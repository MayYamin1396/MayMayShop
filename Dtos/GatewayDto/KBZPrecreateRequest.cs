namespace MayMayShop.API.Dtos.GatewayDto
{
    public class KBZPrePaymentRequest
    {
        public KBZPrecreateRequest Request { get; set; }
    }
    public class KBZPrecreateRequest
    {
        public string timestamp { get; set; } 

        public string notify_url { get; set; }

        public string method { get; set; }

        public string nonce_str { get; set; }

        public string sign_type { get; set; }

        public string sign { get; set; }

        public string version { get; set; }

        public BizContent biz_content { get; set; }

    }

    public class BizContent
    {
        public string merch_order_id { get; set; }

        public string merch_code { get; set; }   

        public string appid { get; set; }

        public string trade_type { get; set; }

        public double total_amount { get; set; }

        public string trans_currency { get; set; }
    }
}