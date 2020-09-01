using MayMayShop.API.Models;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.GatewayDto
{
    public class KBZPQueryOrderResponse : ResponseStatus
    {
        public QueryOrderResponse Response { get; set; }
    }

    public class QueryOrderResponse
    {
        public string result { get; set; }
        public string code { get; set; }
        public string msg { get; set; }

        public string merch_order_id { get; set; }

        public string prepay_id { get; set; }

        public string noonce_str { get; set; }

        public string sign_type { get; set; }

        public string sign { get; set; }

        public string total_amount {get;set;}
        public string trans_currency { get; set; }
        public string trade_status {get;set;}
        public string mm_order_id {get;set;}
        public string pay_success_time	{get;set;}
    }
}
