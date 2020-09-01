using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MayMayShop.API.Dtos.MiscellaneousDto;
using MayMayShop.API.Dtos.OrderDto;
using MayMayShop.Dtos.MiscellaneousDto;

namespace  MayMayShop.API.Interfaces.Services
{
    public interface IDeliveryService
    {
         Task<GetCityResponse> GetCity(string token);
         Task<GetTownResponse> GetTownship(int cityId,string token);
         Task<string> GetCityName(string token,int? id=0);
         Task<string> GetTownshipName(string token,int? id=0);
         Task<List<GetDeliveryServiceResponse>> GetDeliveryService(string token);
         Task<GetDeliveryServiceRateResponse> GetDeliveryServiceRate(int deliveryServiceId,int cityId,int townshipId,string token);
        
    }
}