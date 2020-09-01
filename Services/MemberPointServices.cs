using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MayMayShop.API.Const;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.MembershipDto;
using MayMayShop.API.Dtos.UserDto;
using MayMayShop.API.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MayMayShop.API.Services
{
    public class MemberPointServices : IMemberPointServices
    {
        static HttpClient client = new HttpClient();
        public async Task<List<GetConfigMemberPointResponse>> GetConfigMemberPoint(string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                .GetAsync(MayMayShopConst.MEMBERPOINT_SERVICE_PATH + "GetConfigMemberPoint/?applicationConfigId="+MayMayShopConst.APPLICATION_CONFIG_ID);
            
            if(response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<List<GetConfigMemberPointResponse>>(
                    await response.Content.ReadAsStringAsync());
                return data;
            }
            return new List<GetConfigMemberPointResponse>();
        }
        public async Task<GetConfigMemberPointResponse> GetConfigMemberPointById(int id, string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                .GetAsync(MayMayShopConst.MEMBERPOINT_SERVICE_PATH + "GetConfigMemberPointById/?id="+id+"&applicationConfigId="+MayMayShopConst.APPLICATION_CONFIG_ID);
            
            if(response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<GetConfigMemberPointResponse>(
                    await response.Content.ReadAsStringAsync());
                return data;
            }
            return new GetConfigMemberPointResponse();
        }
        public async Task<ResponseStatus> ReceivedMemberPoint(ReceivedMemberPointRequest request, string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);  

            var json = JsonConvert.SerializeObject(request);
            var dataToSend = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(MayMayShopConst.MEMBERPOINT_SERVICE_PATH + "ReceivedMemberPoint/", dataToSend);

            if(response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<ResponseStatus>(
                    await response.Content.ReadAsStringAsync());
                return data;
            }
            return new ResponseStatus(){StatusCode=StatusCodes.Status404NotFound};
        }
        public async Task<GetMyOwnPointResponse> GetMyOwnPoint(GetMyOwnPointRequest request,string token)
        {
            var data=new GetMyOwnPointResponse(){
                UserId=request.UserId,
                TotalPoint=0
            };
             token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                .GetAsync(MayMayShopConst.MEMBERPOINT_SERVICE_PATH + "GetMyOwnPoint/?userId="+request.UserId+"&applicationConfigId="+MayMayShopConst.APPLICATION_CONFIG_ID);
            
            if(response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<GetMyOwnPointResponse>(
                    await response.Content.ReadAsStringAsync());
                    if(res!=null)
                    {
                        data=res;
                    }
                return data;
            }
            return data;
        }
        public async Task<ResponseStatus> RedemptionMemberPoint(RedemptionMemberPointRequest request, string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);  

            var json = JsonConvert.SerializeObject(request);
            var dataToSend = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(MayMayShopConst.MEMBERPOINT_SERVICE_PATH + "RedemptionMemberPoint/", dataToSend);

            if(response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<ResponseStatus>(
                    await response.Content.ReadAsStringAsync());
                return data;
            }
            return new ResponseStatus(){StatusCode=StatusCodes.Status404NotFound};
        }
    }
}
