using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MayMayShop.API.Helpers;
using MayMayShop.API.Interfaces.Repos;
using log4net;
using System.Threading.Tasks;
using MayMayShop.API.Dtos.ProductDto;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using MayMayShop.API.Const;
using DeviceDetectorNET.Parser;
using MayMayShop.API.Dtos.ReportDto;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ActionActivity))]
    [ServiceFilter(typeof(ActionActivityLog))]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _repo;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ReportController(IReportRepository repo)
        {
            _repo = repo;
        }
        [HttpGet("GetActivityLog")]
        public async Task<IActionResult> GetActivityLog([FromQuery]GetActivityLogRequest request)
        {            
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _repo.GetActivityLog(request,token);
                return Ok(response);
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                log.Error(controllerName + " > " + actionName + " : " + DateTime.Now.ToString() + " => " +  e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet("GetRevenue")]
        public async Task<IActionResult> GetRevenue([FromQuery]GetRevenueRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _repo.GetRevenue(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                log.Error(controllerName + " > " + actionName + " : " + DateTime.Now.ToString() + " => " +  e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpGet("GetSearchKeyword")]
        public async Task<IActionResult> GetSearchKeyword([FromQuery]GetSearchKeywordRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _repo.GetSearchKeyword(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                log.Error(controllerName + " > " + actionName + " : " + DateTime.Now.ToString() + " => " +  e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("NewRegisterCount")]
        public async Task<IActionResult> NewRegisterCount(NewRegisterCountRequest request)
        {
            try
            {
                var platform=3;
                try{
                    #region Platform 
                    DeviceDetectorNET.DeviceDetector.SetVersionTruncation(VersionTruncation.VERSION_TRUNCATION_NONE);
                    var userAgent = Request.Headers["User-Agent"];
                    var result = DeviceDetectorNET.DeviceDetector.GetInfoFromUserAgent(userAgent);
                    var agent = result.Success ? result.ToString().Replace(Environment.NewLine, "<br/>") : "Unknown";
                    var agentArray=agent.Split("<br/>");                    
                    if(MayMayShopConst.AndroidDevice.Contains(agentArray[7].Replace("Name: ","").Replace(";","").Trim()))
                    {
                        platform=1; //Android
                    }
                    else if(MayMayShopConst.IosDevice.Contains(agentArray[7].Replace("Name: ","").Replace(";","").Trim()))
                    {
                        platform=2; //IOS
                    }
                    else{
                        platform=3; //Web                
                    } 
                    #endregion
                }
                catch{
                }

                var response = await _repo.NewRegisterCount(request,platform);
                return Ok(response);
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                log.Error(controllerName + " > " + actionName + " : " + DateTime.Now.ToString() + " => " +  e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    
        [HttpGet("GetProductSearch")]
        public async Task<IActionResult> GetProductSearch([FromQuery]GetProductSearchRequest request)
        {
            try
            {
                var response = await _repo.GetProductSearch(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                log.Error(controllerName + " > " + actionName + " : " + DateTime.Now.ToString() + " => " +  e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}