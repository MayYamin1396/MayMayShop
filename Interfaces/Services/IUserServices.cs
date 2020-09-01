using System.Collections.Generic;
using System.Threading.Tasks;
using MayMayShop.API.Dtos.UserDto;

namespace MayMayShop.API.Interfaces.Services
{
    public interface IUserServices
    {
        Task<GetUserInfoResponse> GetUserInfo(int userId, string token);        
        Task<List<GetAllSellerUserIdResponse>> GetAllSellerUserId(string token);       
    }
}