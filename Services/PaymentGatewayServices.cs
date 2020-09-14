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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

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

        //-----------------WAVE PAY-------------------//

        public async Task<PostOrderByWavePayResponse> WavePayPrecreate(string TransactionId,double NetAmount, List<ProductItem> Items, string payment_description)
        {
            var result = new PostOrderByWavePayResponse();

            var request = new WavePrecreateRequest{
                time_to_live_in_seconds = MayMayShopConst.WAVE_TIME_TO_LIVE_IN_SECONDS,
                merchant_id = MayMayShopConst.WAVE_MERCHANT_ID,
                order_id = TransactionId,
                merchant_reference_id = TransactionId,
                frontend_result_url = MayMayShopConst.WAVE_FRONTEND_RESULT_URL,
                backend_result_url = MayMayShopConst.WAVE_BACKEND_RESULT_URL,
                amount = Convert.ToInt32(NetAmount),
                payment_description = payment_description,
                merchant_name = MayMayShopConst.WAVE_MERCHANT_NAME,
                items = null,
                hash = null,
            };

            request.hash = GenerateSHA256Hash_WaveOrder(request);

            var itms = JsonConvert.SerializeObject(Items);
            request.items = itms;

            string json = JsonConvert.SerializeObject(request,Formatting.Indented);

            log.Info("Request => " + json);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            var response = await client.PostAsync(MayMayShopConst.WAVE_URI, content);
            log.Info("Response => " + response);

            result = JsonConvert
                .DeserializeObject<PostOrderByWavePayResponse>(response.Content.ReadAsStringAsync().Result);
            if (result != null)
            {
                result.WaveUrl = MayMayShopConst.WAVE_AUTHENTICATE_URI+"transaction_id="+result.transaction_id;
            }
            
            log.Info("Response => " + JsonConvert.SerializeObject(result));    
            return result;
        }
        private string GenerateSHA256Hash_WaveOrder(WavePrecreateRequest req)
        {

            string[] strArry = {       
                                    (req.time_to_live_in_seconds).ToString(),
                                    req.merchant_id,
                                    req.order_id,
                                    (req.amount).ToString(),
                                    req.backend_result_url,
                                    req.merchant_reference_id
                                };
            var data=String.Join("", strArry.Where(s => !String.IsNullOrEmpty(s))) ;

            var hash = GetSHA256Wave(data);

            return hash;
        }

        private string GetSHA256Wave(string strArray) 
        {
            var key = MayMayShopConst.WAVE_SECRET_KEY;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] keyBytes = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(strArray);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);

            byte[] bytes = cryptographer.ComputeHash(messageBytes);

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

    }
}