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
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MayMayShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]    
    
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        //[ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        //[ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]
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
         [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]
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

        [HttpGet("GetPolicyById")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> GetPolicyById(int Id)
        {
            try
            {
                var response = await _repo.GetPolicyById(Id);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        
        [HttpPost("CreatePolicy")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> CreatePolicy(CreatePolicyRequest req)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response= await _repo.CreatePolicy(req, currentLoginID);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("UpdatePolicy")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> UpdatePolicy(UpdatePolicyRequest req)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response= await _repo.UpdatePolicy(req, currentLoginID);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("DeletePolicy")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> DeletePolicy(int id)
        {
            try
            {
                
                var response= await _repo.DeletePolicy(id);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("CreateBanner")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> CreateMultipleBanner(CreateMultipleBannerRequest req)
        {
            try
            {
                var response = new ResponseStatus();   
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (req.Banners.Count > 0)
                {
                    if (req.Banners[0].BannerType == 1)
                    {
                        var Banners = _context.Banner.Where(x => x.BannerType == "Banner");
                        _context.Banner.RemoveRange(Banners);
                    }else{
                        var Ads = _context.Banner.Where(x => x.BannerType == "AD");
                        _context.Banner.RemoveRange(Ads);
                    }
                    await _context.SaveChangesAsync();

                    foreach (var item in req.Banners)
                    {
                        ImageUrlResponse img  =new ImageUrlResponse();                
                        img = await _services.UploadToS3NoFixedSize(item.ImageRequest.ImageContent, item.ImageRequest.Extension,MayMayShopConst.AWS_PRODUCT_PATH);
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
         [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        //[ServiceFilter(typeof(ActionActivityLog))]
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
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]
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
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
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
        
        #region Activity Log API
        [HttpGet("GetLastActiveByUserId")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> GetLastActiveByUserId(int userId)
        {
            try
            {
                var response = await _repo.GetLastActiveByUserId(userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        #endregion

        #region Payment service
       
        [HttpGet("GetPaymentServiceForSeller")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> GetPaymentServiceForSeller()
        {
            try
            {
                var response = await _repo.GetPaymentServiceForSeller();
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetPaymentServiceDetail")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> GetPaymentServiceDetail(int paymentServiceId)
        {
            try
            {
                var response = await _repo.GetPaymentServiceDetail(paymentServiceId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("UpdatePaymentService")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> UpdatePaymentService(UpdatePaymentServiceRequest request)
        {
            try
            {
                var response = await _repo.UpdatePaymentService(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        
        [HttpGet("GetBankListForSeller")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> GetBankListForSeller()
        {
            try
            {
                var response = await _repo.GetBankListForSeller();
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetBankDetail")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> GetBankDetail(int id)
        {
            try
            {
                var response = await _repo.GetBankDetail(id);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("UpdateBank")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> UpdateBank(UpdateBankRequest request)
        {
            try
            {
                var response = await _repo.UpdateBank(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
       

        #endregion
        
        #region Category Icon
        [HttpPost("CreateCategoryIcon")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> CreateCategoryIcon([FromForm]CreateCategoryIconRequest request)
        {
            try
            {  
                if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    var imgList = new List<ImageUrlResponse>();
                    foreach (IFormFile file in Request.Form.Files)
                    {
                        ImageUrlResponse imageUrlRes = new ImageUrlResponse();
                        using (var ms = new MemoryStream())
                        {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        string imgStr = Convert.ToBase64String(fileBytes);
                        
                        ImageUrlResponse img  =new ImageUrlResponse(); 
                        img =(await _services.UploadToS3(imgStr,"png", "category"));   
                        imgList.Add(img);
                        }
                    }
                    var oldIcon=await _repo.GetCategoryIcon();
                    foreach(var image in oldIcon)
                    {
                        await _services.DeleteFromS3(image.Url,image.Url);                         
                    }   

                    var response = await _repo.CreateCategoryIcon(request,imgList);
                    return Ok(response);

                }

                else{
                    return StatusCode(StatusCodes.Status500InternalServerError,"File not found!");
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        #endregion

        [HttpPost("GetPaymentServiceForBuyer")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaymentServiceForBuyer()
        {
        
            try
            {
                var response = await _repo.GetPaymentServiceForBuyer();

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        //Brand 

        [HttpPost("AddBrand")]
        public async Task<IActionResult> AddBrand(AddBrandRequest request)
        {
            try
            {
                ImageUrlResponse img  =new ImageUrlResponse();                
                img = await _services.UploadToS3NoFixedSize(request.CoverImage.ImageContent, request.CoverImage.Extension,MayMayShopConst.AWS_BRAND_BANNER_PATH);

                ImageUrlResponse logo  =new ImageUrlResponse();                
                logo = await _services.UploadToS3(request.LogoImage.ImageContent, request.LogoImage.Extension,MayMayShopConst.AWS_BRAND_LOGO_PATH);

                var response= await _repo.AddBrand(request, img.ImgPath, logo.ImgPath);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("UpdateBrand")]
        public async Task<IActionResult> UpdateBrand(UpdateBrandRequest request)
        {
            try
            {
                var brand = await _context.Brand.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
                await _services.DeleteFromS3(brand.Url,brand.Url);
                await _services.DeleteFromS3(brand.LogoUrl,brand.LogoUrl);

                ImageUrlResponse img  =new ImageUrlResponse();                
                img = await _services.UploadToS3NoFixedSize(request.CoverImage.ImageContent, request.CoverImage.Extension,MayMayShopConst.AWS_BRAND_BANNER_PATH);

                ImageUrlResponse logo  =new ImageUrlResponse();                
                logo = await _services.UploadToS3(request.LogoImage.ImageContent, request.LogoImage.Extension,MayMayShopConst.AWS_BRAND_LOGO_PATH);

                var response= await _repo.UpdateBrand(request, img.ImgPath, logo.ImgPath);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("DeleteBrand")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                
                var response= await _repo.DeleteBrand(id);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetBrand")]
        public async Task<IActionResult> GetBrand()
        {
            try
            {
                var response = await _repo.GetBrand();
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