using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DeviceDetectorNET.Parser;
using MayMayShop.API.Const;
using MayMayShop.API.Context;
using MayMayShop.API.Models;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MayMayShop.API.Helpers
{
    public class ActionActivityLog : IAsyncActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MayMayShopContext _context;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionActivityLog(IHttpContextAccessor httpContextAccessor,MayMayShopContext context)
        {
            _httpContextAccessor = httpContextAccessor; 
            _context=context;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {            
            var userId=0;
            try{
                 userId=int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch{
            }
            
            try{

            #region Platform 
            DeviceDetectorNET.DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
            var userAgent = context.HttpContext.Request.Headers["User-Agent"];
            var result = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent);
            var agent = result.Success ? result.ToString().Replace(Environment.NewLine, "<br/>") : "Unknown";
            var agentArray=agent.Split("<br/>");
            var platform=3;
            if(MayMayShopConst.AndroidDevice.Contains(agentArray[7].Replace("Name: ","").Replace(";","").Trim()))
            {
                platform=1;
            }
            else if(MayMayShopConst.IosDevice.Contains(agentArray[7].Replace("Name: ","").Replace(";","").Trim()))
            {
                platform=2;
            }
            else{
                platform=3;                
            } 
            #endregion

            #region Hit count
            
            var ipLog =  _context.ActivityLog.Where(x =>x.Value == userId.ToString() && x.ActivityTypeId == MayMayShopConst.ACTIVITY_TYPE_IP).ToList();
                        
            if(ipLog==null || ipLog.Count()==0)
            {      
                var newActivityLog=new ActivityLog()
                {
                    UserId=userId,
                    ActivityTypeId=MayMayShopConst.ACTIVITY_TYPE_IP,
                    Value=userId.ToString(),
                    CreatedDate=DateTime.Now,
                    CreatedBy=userId,
                    PlatformId=platform
                };
                _context.ActivityLog.Add(newActivityLog);
                _context.SaveChanges();                
            }
            else{
                var dataLog=new List<ActivityLog>();
                var ipID=ipLog.OrderByDescending(x=>x.CreatedDate).FirstOrDefault();
                ipID.CreatedDate=DateTime.Now;
                ipID.CreatedBy=userId;
                dataLog.Add(ipID);
                var ipLogToRemove = ipLog.Except(dataLog).ToList();
                _context.ActivityLog.RemoveRange(ipLogToRemove);
                _context.SaveChanges();
            }
            #endregion
                        
            #region user active log

            var activityLog=_context.ActivityLog.Where(x=>x.UserId==userId && x.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_ACTIVE).OrderByDescending(x=>x.CreatedDate).ToList();
            
            if(activityLog!=null && activityLog.Count()>0)
            {
                var activityLogToUpdate=activityLog.FirstOrDefault();
                activityLogToUpdate.CreatedDate=DateTime.Now;
                activityLogToUpdate.PlatformId=platform;  
                _context.SaveChanges();              
            }
            else{
                var newActivityLog=new ActivityLog()
                {
                    UserId=userId,
                    ActivityTypeId=MayMayShopConst.ACTIVITY_TYPE_ACTIVE,
                    Value="Online",
                    CreatedDate=DateTime.Now,
                    CreatedBy=userId,
                    PlatformId=platform
                };
                _context.ActivityLog.Add(newActivityLog); 
                _context.SaveChanges();
            } 

            var dataActive=new List<ActivityLog>();
            var activeID=activityLog.OrderByDescending(x=>x.CreatedDate).FirstOrDefault();
            dataActive.Add(activeID);
            var activeToRemove = activityLog.Except(dataActive).ToList();
            _context.ActivityLog.RemoveRange(activeToRemove);
            _context.SaveChanges();

            #endregion

            

            }catch(Exception ex){
                log.Error("Try catch error => "+ex.Message+", inner exeption =>"+ex.InnerException.Message);                
            }

            await next();
            return;
        }
    }
}