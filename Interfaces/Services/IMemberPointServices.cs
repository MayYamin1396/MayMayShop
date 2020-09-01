using System.Collections.Generic;
using System.Threading.Tasks;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.MembershipDto;

namespace MayMayShop.API.Interfaces.Services
{
    public interface IMemberPointServices
    {        
        Task<List<GetConfigMemberPointResponse>> GetConfigMemberPoint(string token);
        Task<GetConfigMemberPointResponse> GetConfigMemberPointById(int id, string token);
        Task<ResponseStatus> ReceivedMemberPoint(ReceivedMemberPointRequest request,string token);    
        Task<GetMyOwnPointResponse> GetMyOwnPoint(GetMyOwnPointRequest request,string token);
        Task<ResponseStatus> RedemptionMemberPoint(RedemptionMemberPointRequest request,string token);   
    }
}