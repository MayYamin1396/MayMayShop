namespace MayMayShop.API.Dtos.GatewayDto
{
    public class KBZOrderPaymentRequest
    {
        public KBZQueryOrderRequest Request { get; set; }
    }
    public class KBZQueryOrderRequest
    {
        public int timestamp { get; set; } 

        public string method { get; set; }

        public string nonce_str { get; set; }

        public string sign_type { get; set; }

        public string sign { get; set; }

        public string version { get; set; }

        public QueryOrderBizContent biz_content { get; set; }

    }

    public class QueryOrderBizContent
    { 

        public string appid { get; set; }
        public string merch_code { get; set; }  
        public string merch_order_id { get; set; }
        // public string mm_order_id {get;set;}
    }
}