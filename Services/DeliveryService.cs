using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MayMayShop.API.Const;
using MayMayShop.API.Dtos.MiscellaneousDto;
using MayMayShop.API.Interfaces.Services;
using Newtonsoft.Json;
using MayMayShop.API.Dtos.OrderDto;
using MayMayShop.Dtos.MiscellaneousDto;

namespace MayMayShop.API.Services
{
    public class DeliveryService : IDeliveryService
    {
        static HttpClient client = new HttpClient();
        public async Task<GetCityResponse> GetCity(string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                .GetAsync(MayMayShopConst.DELIVERY_SERVICE_PATH + "GetCity");
            
            if(response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<List<GetCityResponseArry>>(
                                await response.Content.ReadAsStringAsync());
                return new GetCityResponse() { CityList = data };
            }
            return null;
        }
        public async Task<GetTownResponse> GetTownship(int cityId,string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                                        .GetAsync(MayMayShopConst.DELIVERY_SERVICE_PATH + "GetTownship?cityId="+cityId);
            
            if(response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<List<GetTownResponseArry>>(
                                await response.Content.ReadAsStringAsync());
                return new GetTownResponse() {TownList = data};
            }
            return null;
        }
         public async Task<string> GetCityName(string token,int? id=0)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                                        .GetAsync(MayMayShopConst.DELIVERY_SERVICE_PATH + "GetCityName?id="+id);
            
            if(response.IsSuccessStatusCode)
            {
                // var data = JsonConvert.DeserializeObject<string>(
                //                 await response.Content.ReadAsStringAsync());
                var data = await response.Content.ReadAsStringAsync();
                return data;
            }
            return null;
        }
        public async Task<string> GetTownshipName(string token,int? id=0)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                                        .GetAsync(MayMayShopConst.DELIVERY_SERVICE_PATH + "GetTownshipName?id="+id);
            
            if(response.IsSuccessStatusCode)
            {
                // var data = JsonConvert.DeserializeObject<string>(
                //                 await response.Content.ReadAsStringAsync());

                var data = await response.Content.ReadAsStringAsync();
                
                return data;
            }
            return null;
        }
        public async Task<List<GetDeliveryServiceResponse>> GetDeliveryService(string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                                        .GetAsync(MayMayShopConst.DELIVERY_SERVICE_PATH + "GetDeliveryService");
            
            if(response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<List<GetDeliveryServiceResponse>>(
                                await response.Content.ReadAsStringAsync());
                return data;
            }
            return null;
        }
        public async Task<GetDeliveryServiceRateResponse> GetDeliveryServiceRate(int deliveryServiceId,int cityId,int townshipId,string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                                        .GetAsync(MayMayShopConst.DELIVERY_SERVICE_PATH + "GetDeliveryServiceRate?deliveryServiceId="+deliveryServiceId+"&cityId="+cityId+"&townshipId="+townshipId);
            
            if(response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<GetDeliveryServiceRateResponse>(
                                await response.Content.ReadAsStringAsync());
                return data;
            }
            return null;
        }
    }
}