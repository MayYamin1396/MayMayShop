using MayMayShop.API.Models;
using System.Collections.Generic;

namespace MayMayShop.API.Dtos.GatewayDto
{
    public class KBZPrecreateResponse : ResponseStatus
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        public string result { get; set; }

        public string code { get; set; }

        public string msg { get; set; }

        public string merch_order_id { get; set; }

        public string prepay_id { get; set; }

        public string nonce_str { get; set; }

        public string sign_type { get; set; }

        public string sign { get; set; }
    }
}
