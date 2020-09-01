using MayMayShop.API.Const;
using MayMayShop.API.Dtos.GatewayDto;
using System.Text;
using System.Security.Cryptography;
using MayMayShop.API.Interfaces.Services;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using MayMayShop.API.Dtos.OrderDto;
using log4net;

namespace MayMayShop.API.Services
{
    public class PaymentGateWayServices : IPaymentGatewayServices
    {
        static HttpClient client = new HttpClient();

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<PostOrderByKBZPayResponse> KBZPrecreate(string orderId, double totalAmt,int platform)
        {
            var result = new PostOrderByKBZPayResponse();
            var stamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var nonceStr = GenerateNonce(32);

            var tradeType="";
            if(platform==MayMayShopConst.PLATFORM_WEB)
            {
                tradeType=MayMayShopConst.KBZ_GATEWAY_TRADE_TYPE_WEB;
            }
            else{
                tradeType=MayMayShopConst.KBZ_GATEWAY_TRADE_TYPE_MOBILE;
            }
            var request = new KBZPrecreateRequest {
                timestamp = stamp.ToString(),
                nonce_str = nonceStr,
                notify_url = MayMayShopConst.KBZ_GATEWAY_NOTIFY_URL,
                method = MayMayShopConst.KBZ_GATEWAY_METHOD,
                sign_type = MayMayShopConst.KBZ_GATEWAY_SIGN_TYPE,
                version = MayMayShopConst.KBZ_GATEWAY_VERSION,
                biz_content = new BizContent{
                    appid = MayMayShopConst.KBZ_GATEWAY_APP_ID,
                    merch_code = MayMayShopConst.KBZ_GATEWAY_MERCH_CODE,
                    trade_type =tradeType,
                    trans_currency = MayMayShopConst.KBZ_GATEWAY_CURRENCY,
                    merch_order_id = orderId,
                    total_amount = totalAmt,
                },
                sign = null
            };

            request.sign = GenerateSHA256Hash(request);

            var prePaymentRequest = new KBZPrePaymentRequest{
                Request = request
            };

            var json = JsonConvert.SerializeObject(prePaymentRequest);

            log.Info("Request => " + json);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(MayMayShopConst.KBZ_GATEWAY_URI, data);

            result.Precreate = JsonConvert
                .DeserializeObject<KBZPrecreateResponse>(response.Content.ReadAsStringAsync().Result);

            result.NonceStr = nonceStr;
            result.Timestamp = stamp;

            log.Info("Response => " + JsonConvert.SerializeObject(result));    

            return result;
        }

        public async Task<KBZPQueryOrderResponse> KBZQueryOrder(string TransactionId)
        {
            var result = new KBZPQueryOrderResponse();
            var stamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var nonceStr = GenerateNonce(32);

            var request = new KBZQueryOrderRequest {
                timestamp = stamp,
                nonce_str = nonceStr,
                method = MayMayShopConst.KBZ_GATEWAY_QUERYORDER_METHOD,
                sign_type = MayMayShopConst.KBZ_GATEWAY_SIGN_TYPE,
                version = MayMayShopConst.KBZ_GATEWAY_QUERYORDER_VERSION,
                biz_content = new QueryOrderBizContent{
                    appid = MayMayShopConst.KBZ_GATEWAY_APP_ID,
                    merch_code = MayMayShopConst.KBZ_GATEWAY_MERCH_CODE,
                    merch_order_id = TransactionId
                },
                sign = null
            };

            request.sign = GenerateSHA256Hash_Order(request);

            var prePaymentRequest = new KBZOrderPaymentRequest{
                Request = request
            };

            var json = JsonConvert.SerializeObject(prePaymentRequest);

            log.Info("Request => " + json);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(MayMayShopConst.KBZ_GATEWAY_QUERYORDER_URI, data);

            log.Info("Response => " + response);
            result = JsonConvert
                .DeserializeObject<KBZPQueryOrderResponse>(response.Content.ReadAsStringAsync().Result);
            
            log.Info("Response => " + JsonConvert.SerializeObject(result));    
            return result;
        }

        private string GenerateSHA256Hash(KBZPrecreateRequest req)
        {
            var ret = new StringBuilder();

            ret.Append("appid=" + req.biz_content.appid + "&");
            ret.Append("merch_code=" + req.biz_content.merch_code + "&");
            ret.Append("merch_order_id=" + req.biz_content.merch_order_id + "&");
            ret.Append("method=" + req.method + "&");
            ret.Append("nonce_str=" + req.nonce_str + "&");
            ret.Append("notify_url=" + req.notify_url + "&");
            ret.Append("timestamp=" + req.timestamp + "&");
            ret.Append("total_amount=" + req.biz_content.total_amount + "&");
            ret.Append("trade_type=" + req.biz_content.trade_type + "&");
            ret.Append("trans_currency=" + req.biz_content.trans_currency + "&");
            ret.Append("version=" + req.version + "&");
            ret.Append("key=" + MayMayShopConst.KBZ_GATEWAY_KEY);

            var hash = GetSHA256(ret.ToString());

            log.Info(hash.ToUpper());

            return hash.ToUpper();
        }   

        private string GenerateSHA256Hash_Order(KBZQueryOrderRequest req)
        {
            var ret = new StringBuilder();

            ret.Append("appid=" + req.biz_content.appid + "&");
            ret.Append("merch_code=" + req.biz_content.merch_code + "&");
            ret.Append("merch_order_id=" + req.biz_content.merch_order_id + "&");
            ret.Append("method=" + req.method + "&");
            ret.Append("nonce_str=" + req.nonce_str + "&");
            ret.Append("timestamp=" + req.timestamp + "&");
            ret.Append("version=" + req.version + "&");
            ret.Append("key=" + MayMayShopConst.KBZ_GATEWAY_KEY);

            var abc = ret.ToString();

            var hash = GetSHA256(ret.ToString());

            return hash;
        }

        private string GetSHA256(string text) 
        {
            byte[] message = Encoding.UTF8.GetBytes(text);

            SHA256Managed hashString = new SHA256Managed();

            byte[] hashValue = hashString.ComputeHash(message);
            string hex = "";
            foreach (var x in hashValue){
                hex += string.Format("{0:x2}", x);
            }
            return hex;
        }

        private string GenerateNonce(int length)
        {
            Random random = new Random();
            var validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var nonceString = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                nonceString.Append(validChars[random.Next(0, validChars.Length - 1)]);
            }

            return nonceString.ToString().ToUpper();
        }
    }
}