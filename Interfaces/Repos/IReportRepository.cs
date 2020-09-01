using System.Collections.Generic;
using System.Threading.Tasks;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.ProductDto;
using MayMayShop.API.Dtos.ReportDto;
using MayMayShop.API.Models;

namespace MayMayShop.API.Interfaces.Repos
{
    public interface IReportRepository
    {
         Task<List<GetActivityLogResponse>> GetActivityLog(GetActivityLogRequest request,string token);
         Task<GetRevenueResponse> GetRevenue(GetRevenueRequest request);
         Task<GetSearchKeywordResponse> GetSearchKeyword(GetSearchKeywordRequest request);
         Task<ResponseStatus> NewRegisterCount(NewRegisterCountRequest request,int platform);
         Task<List<GetProductSearchResponse>> GetProductSearch(GetProductSearchRequest request);
    }
}