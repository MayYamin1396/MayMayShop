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
using MayMayShop.API.Dtos.FacebookDto;

namespace Ecommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    public class FacebookController : ControllerBase
    {
        private readonly IFacebookRepository _repo;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public FacebookController(IFacebookRepository repo)
        {
            _repo = repo;
        }
        [HttpGet("GetMainCategory")]
        public async Task<IActionResult> GetMainCategory([FromQuery]FBGetMainCategoryRequest request)
        {            
            try
            {
                var response = await _repo.GetMainCategory(request);
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

        [HttpGet("GetProductListByMainCategory")]
        public async Task<IActionResult> GetProductListByMainCategory([FromQuery]FBGetProductListByMainCategoryRequest request)
        {            
            try
            {
                var response = await _repo.GetProductListByMainCategory(request);
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
        
        [HttpGet("GetLatestProductList")]
        public async Task<IActionResult> GetLatestProductList([FromQuery]FBGetLatestProductListRequest request)
        {            
            try
            {
                var response = await _repo.GetLatestProductList(request);
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

        [HttpGet("GetPopularProductList")]
        public async Task<IActionResult> GetPopularProductList([FromQuery]FBGetPopularProductListRequest request)
        {            
            try
            {
                var response = await _repo.GetPopularProductList(request);
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

        [HttpGet("GetPromotionProductList")]
        public async Task<IActionResult> GetPromotionProductList([FromQuery]FBGetPromotionProductListRequest request)
        {            
            try
            {
                var response = await _repo.GetPromotionProductList(request);
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