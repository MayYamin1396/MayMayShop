using Microsoft.AspNetCore.Mvc;
using MayMayShop.API.Interfaces.Repos;
using AutoMapper;
using MayMayShop.API.Interfaces.Services;
using DinkToPdf.Contracts;
using log4net;
using Microsoft.AspNetCore.Authorization;
using MayMayShop.API.Helpers;
using MayMayShop.API.Dtos.OrderDto;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using GemBox.Document;
using System.Net.Http;
using System.Net;
using MayMayShop.API.Const;
using System.Linq;
using DeviceDetectorNET.Parser;

namespace MayMayShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ActionActivity))]
    // [ServiceFilter(typeof(ActionActivityLog))]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repo;

        private readonly IMapper _mapper;

        private readonly IMayMayShopServices _services;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IConverter _converter;

        public OrderController(IOrderRepository repo, IMapper mapper, IMayMayShopServices services,IConverter converter)
        {
            _repo = repo;
            _mapper = mapper;
            _services = services;
            _converter = converter;
        }
   
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
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
                var response = await _repo.AddToCart(request,userId,platform);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(response);
                }
                else{
                     return StatusCode(response.StatusCode,response);
                }               

            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(RemoveFromCartRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
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
                var response = await _repo.RemoveFromCart(request,userId,platform);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(response);
                }
                else{
                     return StatusCode(response.StatusCode,response);
                }               

            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetCartDetail")]
        public async Task<IActionResult> GetCartDetail()
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response = await _repo.GetCartDetail(userId,token);
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return Ok(response);
                }
                else{
                     return StatusCode(response.StatusCode,response);
                }               

            }
            catch(Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

         [HttpPost("UpdateDeliveryinfo")]
        public async Task<IActionResult> UpdateDeliveryinfo(UpdateDeliveryInfoRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                string token = Request.Headers["Authorization"];
                var response =await _repo.UpdateDeliveryInfo(request, userId,token);
                 
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("UpdateDeliveryDateAndTime")]
        public async Task<IActionResult> UpdateDeliveryDateAndTime(UpdateDeliveryDateAndTimeRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                string token = Request.Headers["Authorization"];
                var response=await _repo.UpdateDeliveryDateAndTime(request, userId,token);
                
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode);
                }
                return Ok(response);
                
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        
        [HttpGet("GetDeliverySlot")]
        public async Task<IActionResult> GetDeliverySlot([FromQuery]GetDeliverySlotRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.GetDeliverySlot(request,userId);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
       
        [HttpPost("PostOrder")]
        public async Task<IActionResult> PostOrder(PostOrderRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                string token = Request.Headers["Authorization"];

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

                var response= await _repo.PostOrder(request,userId,token,platform);
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
        [HttpPost("PostOrderByKBZPay")]
        public async Task<IActionResult> PostOrderByKBZPay(PostOrderRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                string token = Request.Headers["Authorization"];
                var response= await _repo.PostOrderByKBZPay(request,userId,token);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("CheckKPayStatus")]
        public async Task<IActionResult> CheckKPayStatus(string transactionId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                string token = Request.Headers["Authorization"];

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
                var response = await _repo.CheckKPayStatus(transactionId, userId,token,platform);
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

        [HttpPost("UpdateProductCart")]
        public async Task<IActionResult> UpdateProductCart(UpdateProductCartRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                string token = Request.Headers["Authorization"];
                var response= await _repo.UpdateProductCart(request,userId);
                 if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("GetOrderHistory")]
        public async Task<IActionResult> GetOrderHistory([FromQuery] GetOrderHistoryRequest request)
        {
            try
            {
                var response = await _repo.GetOrderHistory(request);
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

        [HttpGet("GetOrderHistorySeller")]
        public async Task<IActionResult> GetOrderHistorySeller([FromQuery] GetOrderHistorySellerRequest request)
        {
            try
            {              
                var response = await _repo.GetOrderHistorySeller(request);
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
        [HttpGet("GetNotificationBuyer")]
        public async Task<IActionResult> GetNotificationBuyer([FromQuery] GetNotificationRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response = await _repo.GetNotificationBuyer(request,userId, token);
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
        [HttpGet("GetNotificationSeller")]
        public async Task<IActionResult> GetNotificationSeller([FromQuery] GetNotificationRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response = await _repo.GetNotificationSeller(request,userId, token);
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
        [HttpGet("SeenNotification")]
        public async Task<IActionResult> SeenNotification([FromQuery] SeenNotificationRequest request)
        {
            try
            {               
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response = await _repo.SeenNotification(request,userId);
                if (response.StatusCode !=StatusCodes.Status200OK)
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

        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusRequest request)
        {
            try
            {
                var currentUserLogin = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.UpdateOrderStatus(request, currentUserLogin);
                
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("UpdatePaymentStatus")]
        public async Task<IActionResult> UpdatePaymentStatus(UpdatePaymentStatusRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var currentUserLogin = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.UpdatePaymentStatus(request, currentUserLogin, token);
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpPost("UpdateDeliveryServiceStatus")]
        public async Task<IActionResult> UpdateDeliveryServiceStatus(UpdateDeliveryServiceStatusRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var currentUserLogin = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.UpdateDeliveryServiceStatus(request, currentUserLogin,token);
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("SellerOrderCancel")]
        public async Task<IActionResult> SellerOrderCancel(OrderCancelRequest request)
        {
            try
            {
                var currentUserLogin = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.SellerOrderCancel(request, currentUserLogin);
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("BuyerOrderCancel")]
        public async Task<IActionResult> BuyerOrderCancel(OrderCancelRequest request)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var currentUserLogin = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.BuyerOrderCancel(request, currentUserLogin, token);
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("PaymentAgain")]
        public async Task<IActionResult> PaymentAgain(PaymentAgainRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.PaymentAgain(request, userId);
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpPost("ChangeDeliveryAddress")]
        public async Task<IActionResult> ChangeDeliveryAddress(ChangeDeliveryAddressRequest request)
        {
            try
            {
                var response = await _repo.ChangeDeliveryAddress(request);
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpPost("PaymentApprove")]
        public async Task<IActionResult> PaymentApprove(PaymentApproveRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.PaymentApprove(request, userId);
                if (response.StatusCode != StatusCodes.Status200OK)
                {
                    return StatusCode(response.StatusCode, response);
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
       
        [HttpGet("GetOrderDetail")]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var response = await _repo.GetOrderDetail(orderId, token);
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
        
        [HttpGet("GetOrderListByProduct")]
        public async Task<IActionResult> GetOrderListByProduct([FromQuery]GetOrderListByProductRequest request)
        {
            try
            {                
                var response = await _repo.GetOrderListByProduct(request);
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
        [HttpGet("GetOrderListByProductId")]
        public async Task<IActionResult> GetOrderListByProductId([FromQuery]GetOrderListByProductIdRequest request)
        {
            try
            {                
                var response = await _repo.GetOrderListByProductId(request);
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
        [HttpGet("GetVoucherNoSuggestion")]
        public async Task<IActionResult> GetVoucherNoSuggestion([FromQuery] GetVoucherNoSuggestionRequest request)
        {
            try
            {
                var response = await _repo.GetVoucherNoSuggestion(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpGet("GetVoucherNoSuggestionSeller")]
        public async Task<IActionResult> GetVoucherNoSuggestionSeller([FromQuery]GetVoucherNoSuggestionSellerRequest request)
        {
            try
            {
                var response = await _repo.GetVoucherNoSuggestionSeller(request);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }

        [HttpGet("voucherprint")]
        public async Task<IActionResult> VoucherPrint(int id)
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            var html = @"
            <html>
            <style>
            @page {
                size: A5 landscape;
                margin: 6cm 1cm 1cm;
                mso-header-margin: 1cm;
                mso-footer-margin: 1cm;
            }

            body {
                background: #EDEDED;
                border: 1pt solid black;
                padding: 20pt;
            }

            br {
                page-break-before: always;
            }

            p { margin: 0; }
            header { color: #FF0000; text-align: center; }
            main { color: #00B050; }
            footer { color: #0070C0; text-align: right; }
            </style>

            <body>
            <header>
                <p>Header text.</p>
            </header>
            <main>
                <p>First page.</p>
                <br>
                <p>Second page.</p>
                <br>
                <p>Third page.</p>
                <br>
                <p>Fourth page.</p>
            </main>
            <footer>
                <p>Footer text.</p>
                <p>Page <span style='mso-field-code:PAGE'>1</span> of <span style='mso-field-code:NUMPAGES'>1</span></p>
            </footer>
            </body>
            </html>";

        var htmlLoadOptions = new HtmlLoadOptions();
        using (var htmlStream = new MemoryStream(htmlLoadOptions.Encoding.GetBytes(html)))
        {
            // Load input HTML text as stream.
            var document = DocumentModel.Load(htmlStream, htmlLoadOptions);
            // Save output PDF file.
            // document.Save("Output.pdf");
            
            //return File(DocumentModel.Load(htmlStream, htmlLoadOptions), "application/pdf");
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            // Stream document to browser in DOCX format.
            document.Save(responseMessage, "Document.pdf");
            return Ok(responseMessage);
        }

            
        }
        
        [HttpGet("GetVoucher")]
        public async Task<IActionResult> GetVoucher(int orderId)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var response = await _repo.GetVoucher(orderId,token);
                return Ok(response);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError,e.Message);
            }
        }
        [HttpGet("GetPOSVoucher")]
        public async Task<IActionResult> GetPOSVoucher(int orderId)
        {
            try
            {
                string token = Request.Headers["Authorization"];
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var response = await _repo.GetPOSVoucher(orderId,userId,token);
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
