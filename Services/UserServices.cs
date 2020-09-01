using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MayMayShop.API.Const;
using MayMayShop.API.Dtos.UserDto;
using MayMayShop.API.Interfaces.Services;
using Newtonsoft.Json;

namespace MayMayShop.API.Services
{
    public class UserServices : IUserServices
    {
        static HttpClient client = new HttpClient();
        public async Task<GetUserInfoResponse> GetUserInfo(int userId, string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                .GetAsync(MayMayShopConst.USER_SERVICE_PATH + "getuserinfo?userId=" + userId +"&applicationConfigId="+MayMayShopConst.APPLICATION_CONFIG_ID);
            
            if(response.IsSuccessStatusCode)
            {
                var userInfo = JsonConvert.DeserializeObject<GetUserInfoResponse>(
                    await response.Content.ReadAsStringAsync());
                return userInfo;
            }
            return null;
        }
        public async Task<List<GetAllSellerUserIdResponse>> GetAllSellerUserId(string token)
        {
            token = token.Remove(0,7);
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client
                .GetAsync(MayMayShopConst.USER_SERVICE_PATH + "getallselleruserid/?applicationConfigId="+MayMayShopConst.APPLICATION_CONFIG_ID);
            
            if(response.IsSuccessStatusCode)
            {
                var sellerList = JsonConvert.DeserializeObject<List<GetAllSellerUserIdResponse>>(
                    await response.Content.ReadAsStringAsync());
                return sellerList;
            }
            return null;
        }
        
    }
}
