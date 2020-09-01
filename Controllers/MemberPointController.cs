using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.MembershipDto;
using MayMayShop.API.Dtos.OrderDto;
using MayMayShop.API.Helpers;
using MayMayShop.API.Interfaces.Repos;
using MayMayShop.API.Interfaces.Services;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MayMayShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ActionActivity))]
    [ServiceFilter(typeof(ActionActivityLog))]

    public class MemberPointController : ControllerBase
    {
        private readonly IMemberPointRepository _memberPointRepo;
        
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MemberPointController(IMemberPointRepository memberPointRepo)
        {
            _memberPointRepo = memberPointRepo;
        }

        [HttpGet("GetConfigMemberPoint")]
        public async Task<IActionResult> GetConfigMemberPoint()
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _memberPointRepo.GetConfigMemberPoint(token);
                if (response == null || response.Count == 0)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetConfigMemberPointById")]
        public async Task<IActionResult> GetConfigMemberPointById(int id)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _memberPointRepo.GetConfigMemberPointById(id,token);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("CreateProductReward")]
        public async Task<IActionResult> CreateProductReward(CreateProductRewardRequest request)
        {
            try
            {
                var response = await _memberPointRepo.CreateProductReward(request);
                if (response.StatusCode !=StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode,response);
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("UpdateProductReward")]
        public async Task<IActionResult> UpdateProductReward(UpdateProductRewardRequest request)
        {
            try
            {
                var response = await _memberPointRepo.UpdateProductReward(request);
                if (response.StatusCode !=StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode,response);
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetRewardProduct")]
        public async Task<IActionResult> GetRewardProduct([FromQuery]GetRewardProductRequest request)
        {
            try
            {                
                var response = await _memberPointRepo.GetRewardProduct(request);
                if (response == null || response.Count == 0)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpGet("GetRewardProductById")]
        public async Task<IActionResult> GetRewardProductById([FromQuery]GetRewardProductByIdRequest request)
        {
            try
            {                
                var response = await _memberPointRepo.GetRewardProductById(request);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetRewardProductDetail")]
        public async Task<IActionResult> GetRewardProductDetail([FromQuery]GetRewardProductDetailRequest request)
        {
            try
            {       
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);         
                var response = await _memberPointRepo.GetRewardProductDetail(request,userId,token);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        
        [HttpGet("GetCartDetailForReward")]
        public async Task<IActionResult> GetCartDetailForReward(int productId,int skuId)
        {
            try
            {       
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);         
                var response = await _memberPointRepo.GetCartDetailForReward(productId,skuId,userId,token);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("RedeemOrder")]
        public async Task<IActionResult> RedeemOrder(RedeemOrderRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);      
                var response = await _memberPointRepo.RedeemOrder(request,userId,token);
                if (response.StatusCode !=StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode,response);
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        
        [HttpPost("RedeemOrderByKBZPay")]
        public async Task<IActionResult> RedeemOrderByKBZPay(RedeemOrderRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);      
                var response = await _memberPointRepo.RedeemOrderByKBZPay(request,userId,token);
                if (response.StatusCode !=StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode,response);
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetProductCategoryForCreateConfigMemberPoint")]
        public async Task<IActionResult> GetProductCategoryForCreateConfigMemberPoint()
        {
            try
            {       
                string token = Request.Headers["Authorization"];                        
                var response = await _memberPointRepo.GetProductCategoryForCreateConfigMemberPoint(token);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

    }
}