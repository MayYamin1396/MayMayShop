using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MayMayShop.API.Const;
using MayMayShop.API.Context;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.ProductDto;
using MayMayShop.API.Dtos.ReportDto;
using MayMayShop.API.Dtos.UserDto;
using MayMayShop.API.Interfaces.Repos;
using MayMayShop.API.Interfaces.Services;
using MayMayShop.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MayMayShop.API.Repos
{
    public class ReportRepository : IReportRepository
    {
        private readonly MayMayShopContext _context;
        private readonly IUserServices _userServices;
        private readonly IMayMayShopServices _MayMayShopServices;
        public ReportRepository(MayMayShopContext context,IUserServices userServices,IMayMayShopServices MayMayShopServices)
        {
            _context = context;
            _userServices = userServices;
            _MayMayShopServices=MayMayShopServices;
        }

        public async Task<List<GetActivityLogResponse>> GetActivityLog(GetActivityLogRequest request,string token)
        {
            List<GetActivityLogResponse> logList=new List<GetActivityLogResponse>();
            var data=await _context.ActivityLog
                    .OrderByDescending(x=>x.CreatedDate)
                    .ToListAsync();
            foreach (var item in data){

                if(item.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_ACTIVE && ((int)DateTime.Now.Subtract(item.CreatedDate).TotalMinutes<=5))
                {
                    GetUserInfoResponse user=new GetUserInfoResponse();
                    if(item.UserId==0)
                    {
                        user.Name="Anonymous user";
                    }
                    else{
                        user=await _userServices.GetUserInfo(item.UserId,token);
                    }
                    
                    string desc=string.Format("{0} is online",user.Name);
                    
                    GetActivityLogResponse log=new GetActivityLogResponse(){
                        Id=item.Id,
                        UserName=user.Name,
                        ActivityType=_context.ActivityType.Where(x=>x.Id==item.ActivityTypeId).Select(x=>x.Name).SingleOrDefault(),
                        Description=desc,
                        TimeAgo=_MayMayShopServices.GetPrettyDate(item.CreatedDate),
                        Platform = _context.Platform.Where(x=>x.Id==item.PlatformId).Select(x=>x.Name).SingleOrDefault()
                    };
                    if(!logList.Any(x=>x.UserName==log.UserName && x.ActivityType==log.ActivityType))
                    {
                        logList.Add(log);
                    }                    
                }
                
                else if(item.ActivityTypeId!=MayMayShopConst.ACTIVITY_TYPE_ACTIVE && item.ActivityTypeId!=MayMayShopConst.ACTIVITY_TYPE_IP){
                    GetUserInfoResponse user=new GetUserInfoResponse();
                    if(item.UserId==0)
                    {
                        user.Name="Anonymous user";
                    }
                    else{
                        user=await _userServices.GetUserInfo(item.UserId,token);
                    }
                    string desc="";
                    int search=MayMayShopConst.ACTIVITY_TYPE_SEARCH;

                    if(item.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_SEARCH)
                    {
                        desc=string.Format("{0} searched ({1})",user.Name,item.Value);
                    }

                    else if(item.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_ADD_TO_CART)
                    {
                        desc=string.Format("{0} added {1}",user.Name,item.Value);
                    }

                    else if(item.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_REMOVE_FROM_CART)
                    {
                        desc=string.Format("{0} removed ({1})",user.Name,item.Value);
                    }

                    else if(item.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_ORDER)
                    {
                        desc=string.Format("{0} made a purchase {1}",user.Name,item.Value);
                    }

                    else if(item.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_ORDER_CANCEL)
                    {
                        desc="";
                    }

                    else if(item.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_REGISTER)
                    {
                        desc=string.Format("{0} registered an account ",user.Name,item.Value);
                    }
                   
                    GetActivityLogResponse log=new GetActivityLogResponse(){
                        Id=item.Id,
                        UserName=user.Name,
                        ActivityType=_context.ActivityType.Where(x=>x.Id==item.ActivityTypeId).Select(x=>x.Name).SingleOrDefault(),
                        Description=desc,
                        TimeAgo=_MayMayShopServices.GetPrettyDate(item.CreatedDate),
                        Platform = _context.Platform.Where(x=>x.Id==item.PlatformId).Select(x=>x.Name).SingleOrDefault()
                    };
                    logList.Add(log);
                }
                if (logList.Count()==request.Top)
                {
                    break; // get out of the loop
                }                
            }
            return logList;
        }

        public async Task<List<GetProductSearchResponse>> GetProductSearch(GetProductSearchRequest request)
        {
            if(request.SearchType==MayMayShopConst.SEARCH_KEYWORD_WITH_RESULT)
            {
                return await ( from al in _context.ActivityLog
                where al.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_SEARCH
                && al.CreatedDate.Date<=request.ToDate.Date
                && al.CreatedDate.Date>=request.FromDate.Date
                && al.ResultCount>0
                group al by new { al.Value, al.CreatedDate.Date } into g
                select new { Value=g.Key.Value,
                             CreatedDate=g.Key.Date,
                             ResultCount = g.Sum(x => x.ResultCount),
                             NoOfSearch= g.Count()}
                )
                .Skip((request.PageNumber-1))
                .Take(request.PageSize)
                .Select(x=>new GetProductSearchResponse{
                    Name=x.Value,
                    NoOfSearch=x.NoOfSearch,
                    ResultCount=x.ResultCount,
                    SearchDate=x.CreatedDate
                })
                .ToListAsync();
            }
            else if (request.SearchType==MayMayShopConst.SEARCH_KEYWORD_WITHOUT_RESULT)
            {
                return await ( from al in _context.ActivityLog
                where al.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_SEARCH
                && al.CreatedDate.Date<=request.ToDate.Date
                && al.CreatedDate.Date>=request.FromDate.Date
                && (al.ResultCount==null || al.ResultCount<=0)
                group al by new { al.Value, al.CreatedDate.Date } into g
                select new { Value=g.Key.Value,
                             CreatedDate=g.Key.Date,
                             ResultCount = g.Sum(x => x.ResultCount),
                             NoOfSearch= g.Count()}
                )
                .Skip((request.PageNumber-1))
                .Take(request.PageSize)
                .Select(x=>new GetProductSearchResponse{
                    Name=x.Value,
                    NoOfSearch=x.NoOfSearch,
                    ResultCount=x.ResultCount,
                    SearchDate=x.CreatedDate
                })
                .ToListAsync();
            }
            else{
                return null;
            }
            
        }

        public async Task<GetRevenueResponse> GetRevenue(GetRevenueRequest request)
        {
            int totalQty_Android=0;
            double totalPrice_Android=0;
            int totalVisitor_Android=0;

            int totalQty_IOS=0;
            double totalPrice_IOS=0;
            int totalVisitor_IOS=0;

            int totalQty_Web=0;
            double totalPrice_Web=0;
            int totalVisitor_Web=0;

            // Payment Status for check and fail
            int[] paymentStatus=new int[2]{1,3};

            // Get order id that status are check and fail
            int[] paymentInfoOrderId=await _context.OrderPaymentInfo
                            .Where(x=>x.TransactionDate.Date>=request.FromDate.Date
                            && x.TransactionDate.Date<=request.ToDate.Date
                            && paymentStatus.Contains(x.PaymentServiceId))
                            .Select(x=>x.OrderId)
                            .ToArrayAsync();

            // Get order list that status are not in check, fail, and reject.
            var orderList=await _context.Order
                        .Include(x=>x.OrderDetail)
                        .Where(x=>x.OrderDate.Date>=request.FromDate.Date
                        && x.OrderDate.Date<=request.ToDate.Date
                        && x.OrderStatusId!=5
                        && !paymentInfoOrderId.Contains(x.Id))
                        .ToListAsync();

            totalVisitor_Android=await _context.ActivityLog.Where(x=>x.CreatedDate.Date>=request.FromDate.Date
                            && x.CreatedDate.Date<=request.ToDate.Date
                            && x.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_IP
                            && x.PlatformId==MayMayShopConst.PLATFORM_ANDROID).CountAsync();
            
            totalVisitor_IOS=await _context.ActivityLog.Where(x=>x.CreatedDate.Date>=request.FromDate.Date
                            && x.CreatedDate.Date<=request.ToDate.Date
                             && x.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_IP
                             && x.PlatformId==MayMayShopConst.PLATFORM_IOS).CountAsync();

            totalVisitor_Web=await _context.ActivityLog.Where(x=>x.CreatedDate.Date>=request.FromDate.Date
                            && x.CreatedDate.Date<=request.ToDate.Date
                            && x.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_IP
                            && x.PlatformId==MayMayShopConst.PLATFORM_WEB).CountAsync();
            
            totalPrice_Android=orderList.Where(x=>x.PlatformId==MayMayShopConst.PLATFORM_ANDROID).Select(x=>x.TotalAmt).Sum();
            totalPrice_IOS=orderList.Where(x=>x.PlatformId==MayMayShopConst.PLATFORM_IOS).Select(x=>x.TotalAmt).Sum();
            totalPrice_Web=orderList.Where(x=>x.PlatformId==MayMayShopConst.PLATFORM_WEB).Select(x=>x.TotalAmt).Sum();

            int[] orderId_Android=orderList.Where(x=>x.PlatformId==MayMayShopConst.PLATFORM_ANDROID).Select(x=>x.Id).ToArray();
            int[] orderId_IOS=orderList.Where(x=>x.PlatformId==MayMayShopConst.PLATFORM_IOS).Select(x=>x.Id).ToArray();
            int[] orderId_Web=orderList.Where(x=>x.PlatformId==MayMayShopConst.PLATFORM_WEB).Select(x=>x.Id).ToArray();

            totalQty_Android=_context.OrderDetail.Where(x=>orderId_Android.Contains(x.OrderId)).Select(x=>x.Qty).Sum();
            totalQty_IOS=_context.OrderDetail.Where(x=>orderId_IOS.Contains(x.OrderId)).Select(x=>x.Qty).Sum();
            totalQty_Web=_context.OrderDetail.Where(x=>orderId_Web.Contains(x.OrderId)).Select(x=>x.Qty).Sum();

            GetRevenueResponse response=new GetRevenueResponse(){
                Revenue_Android=totalPrice_Android,
                SoldItem_Android=totalQty_Android,
                TotalVisitor_Android=totalVisitor_Android,
                Revenue_IOS=totalPrice_IOS,
                SoldItem_IOS=totalQty_IOS,
                TotalVisitor_IOS=totalVisitor_IOS,
                Revenue_Web=totalPrice_Web,
                SoldItem_Web=totalQty_Web,
                TotalVisitor_Web=totalVisitor_Web,
            };
            return response;
        }

        public async Task<GetSearchKeywordResponse> GetSearchKeyword(GetSearchKeywordRequest request)
        {
            int dateDiff=(request.ToDate.Date - request.FromDate.Date).Days + 1;

            var topSearchKeyword =await  (from trn in _context.SearchKeywordTrns
                                    where trn.CreatedDate.Date>=request.FromDate.Date
                                    && trn.CreatedDate.Date<=request.ToDate.Date
                                    group trn by trn.SearchKeywordId into newGroup
                                    orderby newGroup.Key
                                    select new TopKeyword{
                                        Name=_context.SearchKeyword.Where(x=>x.Id==newGroup.Key).Select(x=>x.Name).SingleOrDefault(),
                                        KeywordId=newGroup.Key,
                                        Count=newGroup.Sum(x=>x.Count),
                                    })
                                    .OrderByDescending(x=>x.Count)
                                    .Take(request.Top) 
                                    .ToListAsync();

            int totalCount=topSearchKeyword.Sum(x=>x.Count);
            
            foreach(var top in topSearchKeyword)
            {
                top.Percent= (((double)top.Count / totalCount) * 100);
            }

            var mostSearchInDay =await  (from trn in _context.SearchKeywordTrns
                                    where trn.CreatedDate.Date>=request.FromDate.Date
                                    && trn.CreatedDate.Date<=request.ToDate.Date
                                    group trn by trn.CreatedDate.Date into newGroup
                                    orderby newGroup.Key
                                    select new TopKeyword{  
                                        Count=newGroup.Sum(x=>x.Count),
                                    })
                                    .OrderByDescending(x=>x.Count)
                                    .Take(request.Top)  
                                    .Select(x=>x.Count)                                
                                    .FirstOrDefaultAsync();

            var response =new GetSearchKeywordResponse(){
                TopKeyword=topSearchKeyword.OrderByDescending(x=>x.Percent).ToList(),
                NoOfSearch=totalCount,
                MostSearch=mostSearchInDay,
                AverageSearch=((double)totalCount/dateDiff).ToString("0.00"),
            };
            return response;
            
        }

        public async Task<ResponseStatus> NewRegisterCount(NewRegisterCountRequest request,int platform)
        {
            if(!await _context.ActivityLog.AnyAsync(x=>x.UserId==request.UserId && x.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_REGISTER))
            {ActivityLog log=new ActivityLog(){
                UserId=request.UserId,
                ActivityTypeId=MayMayShopConst.ACTIVITY_TYPE_REGISTER,
                Value="",
                CreatedDate=DateTime.Now,
                CreatedBy=request.UserId,
                PlatformId=platform
            };
            _context.ActivityLog.Add(log);
            await _context.SaveChangesAsync();
            }
            
            return new ResponseStatus(){StatusCode=200,Message="Successfully registered."};
        }
    
    }
}