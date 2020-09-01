using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MayMayShop.API.Const;
using MayMayShop.API.Context;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.MiscellaneousDto;
using MayMayShop.API.Helpers;
using MayMayShop.API.Interfaces.Repos;
using MayMayShop.API.Interfaces.Services;
using MayMayShop.Dtos.MiscellaneousDto;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MayMayShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ActionActivity))]
    // [ServiceFilter(typeof(ActionActivityLog))]

    public class MiscellaneousController : ControllerBase
    {
        private readonly IMiscellaneousRepository _repo;
        private readonly IDeliveryService _deliServices;
        private readonly IMayMayShopServices _services;
        private readonly MayMayShopContext _context;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MiscellaneousController(IMiscellaneousRepository repo,IDeliveryService deliService,IMayMayShopServices services, MayMayShopContext context)
        {
            _repo = repo;
            _deliServices=deliService;
            _services=services;
            _context = context;
        }

        [HttpGet("GetCity")]
        public async Task<IActionResult> GetCity()
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _deliServices.GetCity(token);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetTownship")]
        public async Task<IActionResult> GetTownship([FromQuery]GetTownshipRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _deliServices.GetTownship(request.CityId, token);
                return Ok(response);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetMainCategory")]
        public async Task<IActionResult> GetMainCategory()
        {
            try
            {
                var response = await _repo.GetMainCategory();
                return Ok(response);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory([FromQuery]GetSubCategoryRequest request)
        {
            try
            {
                var response = await _repo.GetSubCategory(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("SearchTag")]
        public async Task<IActionResult> SearchTag([FromQuery]SearchTagRequest request)
        {
            try
            {
                var response = await _repo.SearchTag(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetBank")]
        public async Task<IActionResult> GetBank()
        {
            try
            {
                var response = await _repo.GetBank();
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpGet("GetTag")]
        public async Task<IActionResult> GetTag()
        {
            try
            {
                var response = await _repo.GetTag();
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("SearchCategory")]
        public async Task<IActionResult> SearchCategory(string searchText)
        {
            try
            {
                var response = await _repo.SearchCategory(searchText);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetCategoryIcon")]
        public async Task<IActionResult> GetCategoryIcon()
        {
            try
            {
                var response = await _repo.GetCategoryIcon();
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpPost("CreateMainCategory")]
        public async Task<IActionResult> CreateMainCategory(CreateMainCategoryRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.CreateMainCategory(request,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpPost("UpdateMainCategory")]
        public async Task<IActionResult> UpdateMainCategory(UpdateMainCategoryRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.UpdateMainCategory(request,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("DeleteMainCategory")]
        public async Task<IActionResult> DeleteMainCategory(int productCategoryId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.DeleteMainCategory(productCategoryId,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpGet("GetMainCategoryById")]
        public async Task<IActionResult> GetMainCategoryById(int productCategoryId)
        {
            try
            {
                var response = await _repo.GetMainCategoryById(productCategoryId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        
        [HttpPost("CreateSubCategory")]
        public async Task<IActionResult> CreateSubCategory(CreateSubCategoryRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.CreateSubCategory(request,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpPost("UpdateSubCategory")]
        public async Task<IActionResult> UpdateSubCategory(UpdateSubCategoryRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.UpdateSubCategory(request,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("DeleteSubCategory")]
        public async Task<IActionResult> DeleteSubCategory(int productCategoryId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.DeleteSubCategory(productCategoryId,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpGet("GetSubCategoryById")]
        public async Task<IActionResult> GetSubCategoryById(int productCategoryId)
        {
            try
            {
                var response = await _repo.GetSubCategoryById(productCategoryId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

         [HttpPost("CreateVariant")]
        public async Task<IActionResult> CreateVariant(CreateVariantRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.CreateVariant(request,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpPost("UpdateVariant")]
        public async Task<IActionResult> UpdateVariant(UpdateVariantRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.UpdateVariant(request,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("DeleteVariant")]
        public async Task<IActionResult> DeleteVariant(int variantId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.DeleteVariant(variantId,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpGet("GetPolicy")]
        public async Task<IActionResult> GetPolicy()
        {
            try
            {
                var response = await _repo.GetPolicy();
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("CreateBanner")]
        public async Task<IActionResult> CreateBanner(CreateBannerRequest req)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                
                ImageUrlResponse img  =new ImageUrlResponse();                
                img = await _services.UploadToS3(req.ImageRequest.ImageContent, req.ImageRequest.Extension,MayMayShopConst.AWS_BANNER_PATH);

                var response= await _repo.CreateBanner(req, currentLoginID, img.ImgPath);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("CreateMultipleBanner")]
        public async Task<IActionResult> CreateMultipleBanner(CreateMultipleBannerRequest req)
        {
            try
            {
                var response = new ResponseStatus();   
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (req.Banners.Count > 0)
                {
                    var removestatus = _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [Banner]");
                    foreach (var item in req.Banners)
                    {
                        ImageUrlResponse img  =new ImageUrlResponse();                
                        img = await _services.UploadToS3(item.ImageRequest.ImageContent, item.ImageRequest.Extension,MayMayShopConst.AWS_PRODUCT_PATH);
                        CreateBannerRequest banner = new CreateBannerRequest{
                            Name = item.Name,
                            ImageRequest = item.ImageRequest,
                            BannerLinkId = item.BannerLinkId,
                            BannerType = item.BannerType
                        };
                        response= await _repo.CreateBanner(banner, currentLoginID, img.ImgPath);
                    }
                }
                
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(response);
                }else{
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        [HttpPost("UpdateBanner")]
        public async Task<IActionResult> UpdateBanner(UpdateBannerRequest req)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                
                ImageUrlResponse img  =new ImageUrlResponse();

                switch(req.ImageRequest.Action)
                {
                    case "New" : {
                        img = await _services.UploadToS3(req.ImageRequest.ImageContent, req.ImageRequest.Extension,MayMayShopConst.AWS_BANNER_PATH);
                        img.Action="New";
                    }; break;

                    case "Edit" : {
                        var productImg=await _repo.GetBannerById(req.Id);
                        await _services.DeleteFromS3(productImg.Url,productImg.Url);
                        img = await _services.UploadToS3(req.ImageRequest.ImageContent, req.ImageRequest.Extension,MayMayShopConst.AWS_BANNER_PATH);
                        img.Action="Edit";
                        img.ImageId=req.ImageRequest.ImageId;
                    }; break;

                    case "Delete" : {
                        var productImg=await _repo.GetBannerById(req.Id);
                        await _services.DeleteFromS3(productImg.Url,productImg.Url);                      
                        img.Action="Delete";
                        img.ImageId=req.ImageRequest.ImageId;
                    }; break;
                } 
                                   
                var response= await _repo.UpdateBanner(req, currentLoginID, img);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

         [HttpPost("DeleteBanner")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                
                var response= await _repo.DeleteBanner(id, currentLoginID);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        
        [HttpGet("GetBannerById")]
        public async Task<IActionResult> GetBannerById(int id)
        {
            try
            {
                var response = await _repo.GetBannerById(id);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpGet("GetBannerList")]
        public async Task<IActionResult> GetBannerList(int bannerType)
        {
            try
            {
                var response = await _repo.GetBannerList(bannerType);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpGet("GetBannerLink")]
        public async Task<IActionResult> GetBannerLink()
        {
            try
            {
                var response = await _repo.GetBannerLink();
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpGet("GetDeliveryService")]
        public async Task<IActionResult> GetDeliveryService()
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _deliServices.GetDeliveryService(token);
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