using Microsoft.AspNetCore.Mvc;
using MayMayShop.API.Interfaces.Repos;
using AutoMapper;
using MayMayShop.API.Interfaces.Services;
using MayMayShop.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using MayMayShop.API.Dtos.ProductDto;
using MayMayShop.API.Dtos;
using System.Security.Claims;
using MayMayShop.API.Services;
using log4net;
using System;
using MayMayShop.API.Const;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.IO;
using System.Linq;
using OfficeOpenXml.Drawing;
using DeviceDetectorNET.Parser;

namespace MayMayShop.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]    
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repo;

        private readonly IMapper _mapper;

        private readonly IMayMayShopServices _services;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ProductController(IProductRepository repo, IMapper mapper, IMayMayShopServices services)
        {
            _repo = repo;
            _mapper = mapper;
            _services = services;
        }

        [HttpPost("PopulateSku")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]
        public async Task<IActionResult> PopulateSku(PopulateSkuRequest req)
        {
            try
            {
                var trnProductFromRepo = await _repo.CreateDemoProduct(req.ProductCategoryId);
                var resp = await _repo.PopulateSku(req, trnProductFromRepo.Id);
                if (resp == null)
                {
                    return StatusCode(StatusCodes.Status501NotImplemented);
                }
                return Ok(resp);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("CreateProduct")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> CreateProduct(CreateProductRequest req)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var imgList = new List<ImageUrlResponse>();
                foreach(var image in req.ImageList)
                {  
                    ImageUrlResponse img  =new ImageUrlResponse();                
                    img = await _services.UploadToS3(image.ImageContent, image.Extension,MayMayShopConst.AWS_PRODUCT_PATH);
                    imgList.Add(img);
                }
                
                var response= await _repo.CreateProduct(req, imgList, currentLoginID);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
               return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("UpdateProduct")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> UpdateProduct(UpdateProductRequest req)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                                
                // Old logic
                //var imgList = new List<ImageUrlResponse>();
                // foreach(var image in req.ImageList)
                // {  
                //     ImageUrlResponse img  =new ImageUrlResponse();

                //     switch(image.Action)
                //     {
                //         case "New" : {
                //             img = await _services.UploadToS3(image.ImageContent, image.Extension,MayMayShopConst.AWS_PRODUCT_PATH);
                //             img.Action="New";
                //         }; break;

                //         case "Edit" : {
                //             var productImg=await _repo.GetProductImageById(image.ImageId);
                //             await _services.DeleteFromS3(productImg.Url,productImg.ThumbnailUrl);
                //             img = await _services.UploadToS3(image.ImageContent, image.Extension,MayMayShopConst.AWS_PRODUCT_PATH);
                //             img.Action="Edit";
                //             img.ImageId=image.ImageId;
                //         }; break;

                //         case "Delete" : {
                //             var productImg=await _repo.GetProductImageById(image.ImageId);
                //             await _services.DeleteFromS3(productImg.Url,productImg.ThumbnailUrl);                      
                //             img.Action="Delete";
                //             img.ImageId=image.ImageId;
                //         }; break;
                //     }     
                //     imgList.Add(img);
                // }

                // New logic - delete insert
                var imgList = new List<ImageUrlResponse>();
                var oldProductImage=await _repo.GetAllProductImageByProductId(req.ProductId);
                foreach(var image in oldProductImage)
                {
                    ImageUrlResponse img  =new ImageUrlResponse();
                    await _services.DeleteFromS3(image.Url,image.ThumbnailUrl); 
                    img.Action="Delete";
                    img.ImageId=image.Id;
                    imgList.Add(img);
                }                
                foreach(var image in req.ImageList)
                {  
                    ImageUrlResponse img  =new ImageUrlResponse();
                    img = await _services.UploadToS3(image.ImageContent, image.Extension,MayMayShopConst.AWS_PRODUCT_PATH);
                    img.Action="New";   
                    imgList.Add(img);
                }
                
                var response= await _repo.UpdateProduct(req, imgList, currentLoginID);

                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("DeleteSku")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> DeleteSku(DeleteSkuRequest request)
        {
            try
            {
                var response = await _repo.DeleteSku(request);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(response);
                }
                else if (response.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("AddSkuForUpdateProduct")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> AddSkuForUpdateProduct(AddSkuForUpdateProductRequest request)
        {
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.AddSkuForUpdateProduct(request,currentLoginID);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(response);
                }
                else
                {
                    return StatusCode(response.StatusCode, response);
                }
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetProductDetail")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetProductDetail([FromQuery]GetProductDetailRequest request)
        {
            try
            {
                string token ="";
                var userId =0;
                try{
                    token = Request.Headers["Authorization"];
                    userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch(Exception){

                }                

                var response = await _repo.GetProductDetail(request,userId,token);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }
                if (response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    return Unauthorized(response);
                }
                else if (response.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);


            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetLandingProductPromotion")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetLandingProductPromotion([FromQuery]GetLandingProductPromotionRequest request)
        {
            try
            {
                var response = await _repo.GetLandingProductPromotion(request);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }
               
                return Ok(response);

            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetLandingProductLatest")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetLandingProductLatest([FromQuery]GetLandingProductLatestRequest request)
        {
            try
            {
                var response = await _repo.GetLandingProductLatest(request);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);

            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        
        [HttpGet("GetProductByRelatedCategry")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetProductByRelatedCategry([FromQuery]GetProductByRelatedCategryRequest request)
        {
            try
            {
                var response = await _repo.GetProductByRelatedCategry(request);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);

            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }


        [HttpGet("GetProductByRelatedTag")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetProductByRelatedTag([FromQuery]GetProductByRelatedTagRequest request)
        {
            try
            {
                var response = await _repo.GetProductByRelatedTag(request);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);

            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetLandingProductCategory")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetLandingProductCategory([FromQuery]GetLandingProductCategoryRequest request)
        {
            try
            {
                var response = await _repo.GetLandingProductCategory(request);
                if (response == null)
                {
                    return Ok(new { message = "No Result Found!" });
                }               
                return Ok(response);

            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("ProductSearch")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> ProductSearch([FromQuery] ProductSearchRequest request)
        {
            try
            {
                var userId=0;
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
                    userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch{
                }
                var response = await _repo.ProductSearch(request,userId,platform);
                if (response == null || response.Count == 0)
                {
                    return Ok(new { message = "ရှာဖွေတွေ့ရှိချက် မရှိပါ" });
                }
                return Ok(response);
            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
       
        [HttpGet("GetProductList")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetProductList([FromQuery]GetProductListRequest request)
        {
            try
            {
                var response = await _repo.GetProductList(request);
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

        [HttpPut("DeleteProduct")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> DeleteProduct(DeleteProductRequest request)
        {
            try
            {
                var response = await _repo.DeleteProduct(request);
                if (response.StatusCode != StatusCodes.Status200OK)
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

        [HttpGet("GetVariantByCategoryId")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetVariantByCategoryId(int categoryId)
        {
            try
            {
                var variantsFromRepo = await _repo.GetVariantByCategoryId(categoryId);
                return Ok(variantsFromRepo);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("ProductSkuHold")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> ProductSkuHold(ProductSkuHoldRequest req)
        {
            try
            {
                await _repo.ProductSkuHold(req);
                return Ok(StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("GetProductSku")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetProductSku(GetProductSkuRequest req)
        {
            try
            {
                var resp = await _repo.GetProductSku(req);
                if (resp == null)
                {
                    return StatusCode(StatusCodes.Status501NotImplemented);
                }
                return Ok(resp);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("GetProductVariant")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetProductVariant(GetProductSkuRequest req)
        {
            try
            {
                var resp = await _repo.GetProductVariant(req);
                if (resp == null)
                {
                    return StatusCode(StatusCodes.Status501NotImplemented);
                }
                return Ok(resp);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpPost("GetVariantValue")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetVariantValue(GetVariantValueRequest request)
        {
            try
            {
                var response = await _repo.GetVariantValue(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpGet("GetProductNameSuggestion")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetProductNameSuggestion([FromQuery] GetProductNameSuggestionRequest req)
        {
            try
            {
                var response = await _repo.GetProductNameSuggestion(req);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetBestSellingProduct")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetBestSellingProduct([FromQuery]GetBestSellingProductRequest request)
        {
            try
            {
                var response = await _repo.GetBestSellingProduct(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
    
        [HttpPost("UploadProduct")]
        [Authorize]
        [ServiceFilter(typeof(ActionActivity))]
        [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> UploadProduct([FromForm]UploadProductRequest request)
        {           
            try
            {
                var currentLoginID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response=await _repo.UploadProduct(request,currentLoginID);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
            
        }

        [HttpGet("GetAllProductListBuyer")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetAllProductListBuyer([FromQuery]GetAllProductListBuyerRequest request)
        {
            try
            {
                var response = await _repo.GetAllProductListBuyer(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetProductByBrand")]
        // [Authorize]
        // [ServiceFilter(typeof(ActionActivity))]
        // [ServiceFilter(typeof(ActionActivityLog))]

        public async Task<IActionResult> GetProductByBrand([FromQuery]GetProductByBrandRequest request)
        {
            try
            {
                var response = await _repo.GetProductByBrand(request);
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