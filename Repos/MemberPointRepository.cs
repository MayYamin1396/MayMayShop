using MayMayShop.API.Context;
using MayMayShop.API.Interfaces.Repos;
using MayMayShop.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using MayMayShop.Dtos.MiscellaneousDto;
using MayMayShop.API.Dtos.MiscellaneousDto;
using MayMayShop.API.Dtos.MembershipDto;
using MayMayShop.API.Dtos;
using MayMayShop.API.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;
using MayMayShop.API.Dtos.ProductDto;
using MayMayShop.API.Const;
using MayMayShop.API.Dtos.OrderDto;
using Newtonsoft.Json;
using MayMayShop.API.Helpers;
using MayMayShop.API.Dtos.GatewayDto;

namespace MayMayShop.API.Repos
{
    public class MemberPointRepository : IMemberPointRepository
    {
        private readonly MayMayShopContext _context;
        private readonly IProductRepository _productRepo;
        private readonly IUserServices _userServices;
        private readonly IMayMayShopServices _services;
        private readonly IMemberPointServices _memberPointServices;
        private readonly IDeliveryService _deliServices;
        private readonly IPaymentGatewayServices _paymentservices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMiscellaneousRepository _miscellaneousRepo;
        public MemberPointRepository(MayMayShopContext context,
        IMemberPointServices memberPointServices,
        IUserServices userServices,
        IDeliveryService deliService,
        IProductRepository productRepo,
        IMiscellaneousRepository miscellaneousRepo,
        IMayMayShopServices services,
        IPaymentGatewayServices paymentservices,
        IHttpContextAccessor httpContextAccessor)
        {
            _memberPointServices = memberPointServices;
            _context = context;
            _userServices=userServices;
            _deliServices=deliService;
            _productRepo=productRepo;
            _services=services;
            _paymentservices=paymentservices;
            _httpContextAccessor=httpContextAccessor;
            _miscellaneousRepo=miscellaneousRepo;
        }
        public async Task<List<GetConfigMemberPointResponse>> GetConfigMemberPoint(string token)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            var memberPoint=await _memberPointServices.GetConfigMemberPoint(token);
            foreach(var mp in memberPoint)
            {
                foreach(var cate in mp.ProductCategoryList )
                {
                    var category=_context.ProductCategory       
                                            .Where(x=>x.Id==cate.ProductCategoryId)
                                            .SingleOrDefault();
                    if(category!=null)
                    {
                        cate.ProductCategoryName=isZawgyi?Rabbit.Uni2Zg(category.Name):category.Name;
                        cate.Url=category.Url;
                    }
                    else{
                        cate.ProductCategoryName="";
                        cate.Url="";
                    }                    
                }
            }
            return memberPoint;
        }
        public async Task<GetConfigMemberPointResponse> GetConfigMemberPointById(int id, string token)
        {
           bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            var memberPoint=await _memberPointServices.GetConfigMemberPointById(id,token);
            
            if(memberPoint!=null)
            {
            foreach(var cate in memberPoint.ProductCategoryList )
            {
                var category=_context.ProductCategory       
                                        .Where(x=>x.Id==cate.ProductCategoryId)
                                        .SingleOrDefault();
                if(category!=null)
                {
                    cate.ProductCategoryName=isZawgyi?Rabbit.Uni2Zg(category.Name):category.Name;
                    cate.Url=category.Url;
                }
                else{
                    cate.ProductCategoryName="";
                    cate.Url="";
                }                    
            }
            var productCategoryId=memberPoint.ProductCategoryList.Select(x=>x.ProductCategoryId).ToArray();
            var productCategory= await _context.ProductCategory
                    .Where(x=>x.IsDeleted!=true
                    && !productCategoryId.Contains(x.Id)
                    && x.SubCategoryId!=null 
                    && x.SubCategoryId!=0)
                    .Select(x=> new GetConfigMemberPointProductCategory{
                        ProductCategoryId=x.Id,
                        ProductCategoryName=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                        Url=x.Url,
                        ConfigMemberPointId=0,
                        ApplicationConfigId=MayMayShopConst.APPLICATION_CONFIG_ID
                    }).ToListAsync();   
            memberPoint.ProductCategoryList.AddRange(productCategory);  
            }                  
            return memberPoint;
        }        
        public async Task<ResponseStatus> ReceivedMemberPoint(ReceivedMemberPointRequest request, string token)
        {
            return await _memberPointServices.ReceivedMemberPoint(request,token);
        }
        public async Task<ResponseStatus> CreateProductReward(CreateProductRewardRequest request)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            var productPrice=await _context.ProductPrice                            
                            .Where(x=>x.ProductId==request.ProductId)
                            .FirstOrDefaultAsync();

            var existingProductReward=await _context.ProductReward
            .Where(x=>x.ProductId==request.ProductId).FirstOrDefaultAsync();

            if(existingProductReward!=null)
            {
                existingProductReward.ProductId=request.ProductId;
                existingProductReward.Point=request.Point;
                existingProductReward.StartDate=request.StartDate;
                existingProductReward.EndDate=request.EndDate;
                if(request.FixedAmount>0)
                {
                    existingProductReward.RewardAmount=productPrice.Price-request.FixedAmount;
                    existingProductReward.RewardPercent=0;
                    existingProductReward.FixedAmount=request.FixedAmount;
                }
                else if (request.RewardPercent>0){
                    double discountPrice= ((float)((float)request.RewardPercent / (float)100)) * (float)productPrice.Price ;
                    existingProductReward.RewardAmount=productPrice.Price-discountPrice;
                    existingProductReward.RewardPercent=request.RewardPercent;
                    existingProductReward.FixedAmount=0;
                }
                else{
                    existingProductReward.RewardAmount=0;
                    existingProductReward.RewardPercent=0;
                    existingProductReward.FixedAmount=0;
                }

            await _context.SaveChangesAsync();

            var rewardHistory=new MayMayShop.API.Models.ProductRewardHistory(){
              ProductId=request.ProductId,
              StartDate=request.StartDate,
              EndDate=request.EndDate,  
            };
            _context.ProductRewardHistory.Add(rewardHistory);
            await _context.SaveChangesAsync();

            }
            else{

                var productRewardNew=new ProductReward();
                productRewardNew.ProductId=request.ProductId;
                productRewardNew.Point=request.Point;
                productRewardNew.StartDate=request.StartDate;
                productRewardNew.EndDate=request.EndDate;
                if(request.FixedAmount>0)
                {
                    productRewardNew.RewardAmount=productPrice.Price-request.FixedAmount;
                    productRewardNew.RewardPercent=0;
                    productRewardNew.FixedAmount=request.FixedAmount;
                }
                else if (request.RewardPercent>0){
                    double discountPrice= ((float)((float)request.RewardPercent / (float)100)) * (float)productPrice.Price ;
                    productRewardNew.RewardAmount=productPrice.Price-discountPrice;
                    productRewardNew.RewardPercent=request.RewardPercent;
                    productRewardNew.FixedAmount=0;
                }
                else{
                    productRewardNew.RewardAmount=0;
                    productRewardNew.RewardPercent=0;
                    productRewardNew.FixedAmount=0;
                }

            _context.ProductReward.Add(productRewardNew);
            await _context.SaveChangesAsync();

            var rewardHistory=new MayMayShop.API.Models.ProductRewardHistory(){
              ProductId=request.ProductId,
              StartDate=request.StartDate,
              EndDate=request.EndDate,  
            };
            _context.ProductRewardHistory.Add(rewardHistory);
            await _context.SaveChangesAsync();


            }
             
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully added."};
        }
        public async Task<ResponseStatus> UpdateProductReward(UpdateProductRewardRequest request)
        {       
            var productReward=await _context.ProductReward
                            .Where(x=>x.Id==request.Id)
                            .SingleOrDefaultAsync();
            if(productReward!=null)
            {
                 var productPrice=await _context.ProductPrice                            
                            .Where(x=>x.ProductId==request.ProductId)
                            .FirstOrDefaultAsync();


                productReward.Point=request.Point;
                productReward.StartDate=request.StartDate;
                productReward.EndDate=request.EndDate;
                if(request.FixedAmount>0)
                {
                    productReward.RewardAmount=productPrice.Price-request.FixedAmount;
                    productReward.RewardPercent=0;
                    productReward.FixedAmount=request.FixedAmount;
                }
                else if (request.RewardPercent>0){
                    double discountPrice= ((float)((float)request.RewardPercent / (float)100)) * (float)productPrice.Price ;
                    productReward.RewardAmount=productPrice.Price-discountPrice;
                    productReward.RewardPercent=request.RewardPercent;
                    productReward.FixedAmount=0;
                }
                else{
                    productReward.RewardAmount=0;
                    productReward.RewardPercent=0;
                    productReward.FixedAmount=0;
                }

                var history=await _context.ProductRewardHistory.Where(x=>x.ProductId==request.Id && x.StartDate.Date==request.StartDate.Date && x.EndDate.Date==request.EndDate.Date).FirstOrDefaultAsync();
                if(history!=null)
                {
                    history.StartDate=request.StartDate;
                    history.EndDate=request.EndDate;
                }
                await _context.SaveChangesAsync();
            }
            else{
                return new ResponseStatus(){StatusCode=StatusCodes.Status400BadRequest,Message="Record not found!"};
            }
            
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully added."};
        }
        public async Task<List<GetRewardProductResponse>> GetRewardProduct(GetRewardProductRequest request)
        {
           bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.ProductReward
                            .Where(x=>
                            x.StartDate.Date<=DateTime.Now.Date
                            && x.EndDate.Date>=DateTime.Now.Date)
                            .OrderByDescending(x=>x.StartDate)
                            .Select(x=> new GetRewardProductResponse{
                                Id=x.Id,
                                ProductId=x.ProductId,
                                Name=_context.Product.Where(p=>p.Id==x.ProductId).Select(p=>isZawgyi?Rabbit.Uni2Zg(p.Name):p.Name).SingleOrDefault(),
                                Url=_context.ProductImage.Where(p=>p.ProductId==x.ProductId && p.isMain==true).Select(p=>p.Url).SingleOrDefault(),
                                OriginalPrice=_context.ProductPrice.Where(p=>p.ProductId==x.ProductId).OrderByDescending(p=>p.StartDate).Select(p=>p.Price).SingleOrDefault(),
                                StartDate=x.StartDate,
                                EndDate=x.EndDate,
                                Point=x.Point,
                                Qty=_context.ProductSku.Where(sku=>sku.ProductId==x.ProductId).Sum(sku=>sku.Qty),
                                FixedAmount=x.FixedAmount,
                                RewardAmount=x.RewardAmount,
                                RewardPercent=x.RewardPercent
                            })
                            .Skip((request.PageNumber-1)*request.PageSize)
                            .Take(request.PageSize)
                            .ToListAsync();          
        }
        public async Task<GetRewardProductByIdResponse> GetRewardProductById(GetRewardProductByIdRequest request)
        {
             bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            var productReward=await _context.ProductReward
                            .Where(x=>x.Id==request.ProductRewardId)
                            .Select(x=>new GetRewardProductByIdResponse{
                                Id=x.Id,
                                ProductId=x.ProductId,
                                Name=_context.Product.Where(p=>p.Id==x.ProductId).Select(p=>isZawgyi?Rabbit.Uni2Zg(p.Name):p.Name).SingleOrDefault(),
                                Url=_context.ProductImage.Where(p=>p.ProductId==x.ProductId && p.isMain==true).Select(p=>p.Url).SingleOrDefault(),
                                StartDate=x.StartDate,
                                EndDate=x.EndDate,
                                OriginalPrice=_context.ProductPrice.Where(p=>p.ProductId==x.ProductId).OrderByDescending(p=>p.StartDate).Select(p=>p.Price).FirstOrDefault(),
                                RewardAmount=x.RewardAmount,
                                RewardPercent=x.RewardPercent,
                                Point=x.Point,
                                FixedAmount=x.FixedAmount,
                                Qty=_context.ProductSku.Where(p=>p.ProductId==x.ProductId).Sum(p=>p.Qty),
                            })
                            .FirstOrDefaultAsync();

            var productRewardHistory=await _context.ProductRewardHistory
                                    .Where(x=>x.ProductId==productReward.ProductId)
                                    .OrderByDescending(x=>x.EndDate)
                                    .Select(x=>new MayMayShop.API.Dtos.MembershipDto.ProductRewardHistory{
                                        Id=x.Id,
                                        ProductId=x.ProductId,
                                        StartDate=x.StartDate,
                                        EndDate=x.EndDate
                                    })
                                    .ToListAsync(); 
            productReward.ProductRewardHistory= productRewardHistory;
            return  productReward;     
        }
        public async Task<GetRewardProductDetailResponse> GetRewardProductDetail(GetRewardProductDetailRequest request,int currentUserLogin,string token)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

              var product = await _context.Product
                        .Where(x=>x.Id==request.ProductId)
                        .SingleOrDefaultAsync();
              
            var category= await _context.ProductCategory.Where(x=>x.Id==product.ProductCategoryId).SingleOrDefaultAsync();
                                   
            List<GetPrdouctDetailCategoryResponse> proCat=new List<GetPrdouctDetailCategoryResponse>();
            GetPrdouctDetailCategoryResponse subCat= await _context.ProductCategory
                                                    .Where(x=>x.Id==category.Id)
                                                    .Select(x=>new GetPrdouctDetailCategoryResponse{
                                                    ProductCategoryId=x.Id,
                                                    ProductCategoryName=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                                                    Url=x.Url,
                                                    IsMainCategory=false
                                                    }).SingleOrDefaultAsync();
            GetPrdouctDetailCategoryResponse mainCat= await _context.ProductCategory
                                                    .Where(x=>x.Id==category.SubCategoryId)
                                                    .Select(x=>new GetPrdouctDetailCategoryResponse{
                                                    ProductCategoryId=x.Id,
                                                    ProductCategoryName=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                                                    Url=x.Url,
                                                    IsMainCategory=true
                                                    }).SingleOrDefaultAsync();
            proCat.Add(mainCat);
            proCat.Add(subCat);

            var tagIDs= await _context.ProductTag
                    .Where(x=>x.ProductId==request.ProductId)
                    .Select(x=>x.TagId).ToListAsync();
            var tag=await _context.Tag
                    .Where(x=> tagIDs.Contains(x.Id))
                    .Select(x=>x.Name).ToArrayAsync();
            
            var images= await _context.ProductImage
                    .Where(x=>x.ProductId==request.ProductId)
                    .Select(x=>x.Url)
                    .ToArrayAsync();
            
            var req=new GetMyOwnPointRequest(){
                UserId=currentUserLogin
            };
            var myOwnPoint=await _memberPointServices.GetMyOwnPoint(req,token);
            
            var productReward= await _context.ProductReward
                            .Where(x=>x.ProductId==request.ProductId
                            && x.StartDate.Date<=DateTime.Now.Date
                            && x.EndDate.Date>=DateTime.Now.Date)
                            .Select(x=> new GetRewardProductDetailResponse{
                                ProductId=x.ProductId,
                                Name=isZawgyi?Rabbit.Uni2Zg(product.Name):product.Name,
                                Description=isZawgyi?Rabbit.Uni2Zg(product.Description):product.Description,
                                StartDate=x.StartDate,
                                EndDate=x.EndDate,
                                Point=x.Point,
                                FixedAmount=x.FixedAmount,
                                RewardAmount=x.RewardAmount,
                                RewardPercent=x.RewardPercent,
                                ProductTag=tag,
                                ProductCategory=proCat,
                                ProductImage=images,
                                MyOwnPoint=myOwnPoint.TotalPoint,
                                OriginalPrice=_context.ProductPrice
                                            .Where(p=>p.ProductId==x.ProductId)
                                            .Select(p=>p.Price).FirstOrDefault()
                            })
                            .FirstOrDefaultAsync(); 


                    var variantIds = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).Select(s => s.VariantId).Distinct().ToListAsync();

                    productReward.Variant = await _context.Variant.Where(x => x.ProductCategoryId == product.ProductCategoryId  && variantIds.Contains(x.Id))                   
                                    .Select(s => new GetProductDetailVariant
                                    {
                                        VariantId = s.Id,
                                        Name =isZawgyi?Rabbit.Uni2Zg(s.Name):s.Name
                                    }).ToListAsync();

                     var productSkuList = await _context.ProductSku.Where(x => x.ProductId == product.Id).ToListAsync();
                    if (productSkuList.Count > 0)
                    {
                        productReward.SkuValue = new List<GetProductDetailSkuValue>();
                        foreach (var item in productSkuList)
                        {  
                            productReward.Qty += item.Qty;
                            var skuKeys = await _context.ProductSkuValue
                                .Where(x => x.ProductId == item.ProductId && x.SkuId == item.SkuId)
                                .ToListAsync();
                            if (skuKeys.Count > 0)
                            {
                                    var skuValue = await (from psku in _context.ProductSkuValue
                                                        from pvopt in _context.ProductVariantOption
                                                        where psku.ProductId == item.ProductId
                                                        && psku.SkuId == item.SkuId
                                                        && psku.ProductId == pvopt.ProductId
                                                        && psku.VariantId == pvopt.VariantId
                                                        && psku.ValueId == pvopt.ValueId
                                                        select isZawgyi?Rabbit.Uni2Zg(pvopt.ValueName):pvopt.ValueName).ToListAsync();

                                    var skuValeForResp = new GetProductDetailSkuValue
                                    {
                                        SkuId = item.SkuId,
                                        Value = string.Join(",", skuValue),
                                        Qty = item.Qty, 
                                        OriginalPrice=item.Price,
                                    };

                                    if(productReward!=null)
                                    {                                        
                                        skuValeForResp.Point=productReward.Point;

                                       if(productReward.FixedAmount>0)
                                        {
                                            skuValeForResp.RewardAmount=skuValeForResp.OriginalPrice-productReward.FixedAmount;
                                            skuValeForResp.RewardPercent=0;
                                            skuValeForResp.FixedAmount=productReward.FixedAmount;
                                        }
                                        else if (productReward.RewardPercent>0){
                                            double discountPrice= ((float)((float)productReward.RewardPercent / (float)100)) * (float)skuValeForResp.OriginalPrice ;
                                            skuValeForResp.RewardAmount=skuValeForResp.OriginalPrice-discountPrice;
                                            skuValeForResp.RewardPercent=productReward.RewardPercent;
                                            skuValeForResp.FixedAmount=0;
                                        }
                                        else{
                                            skuValeForResp.RewardAmount=0;
                                            skuValeForResp.RewardPercent=0;
                                            skuValeForResp.FixedAmount=0;
                                        }                              
                                        
                                    }
                                    else{
                                        skuValeForResp.RewardAmount=0;
                                        skuValeForResp.RewardPercent=0;
                                        skuValeForResp.FixedAmount=0;
                                    }

                                    productReward.SkuValue.Add(skuValeForResp);                               
                            }
                        }
                    }
                    else
                    {
                        productReward.Qty = 0;
                        productReward.SkuValue = null;
                    }

            var vvreq = new GetVariantValueRequest
            {
                ProductId = request.ProductId,
                CurrentVariantId = productReward.Variant[0].VariantId
            };

            productReward.VariantValues = await _productRepo.GetVariantValue(vvreq);

            return productReward;
        }
        public async Task<ResponseStatus> DeleteProductReward(int id)
        {
            var response=new ResponseStatus();
            var productReward=await _context.ProductReward.Where(x=>x.Id==id)
            .SingleOrDefaultAsync();
            if(productReward!=null)
            {
                _context.ProductReward.Remove(productReward);

                var history=await _context.ProductRewardHistory
                .Where(x=>x.ProductId==productReward.ProductId)
                .ToListAsync();

                _context.ProductRewardHistory.RemoveRange(history);

                await _context.SaveChangesAsync();
                response.StatusCode=StatusCodes.Status200OK;
                response.Message="Success";
            }
            else{
                 response.StatusCode=StatusCodes.Status400BadRequest;
                 response.Message="Record not found!";
            }
            return response;
        }
        public async Task<PostOrderResponse> RedeemOrder(RedeemOrderRequest request,int currentUserLogin,string token)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

                     #region Check qty before order

                    var issueList=new List<ProductIssues>();
                    PostOrderResponse response = new PostOrderResponse();
                    foreach (var item in request.ProductInfo)
                    {
                        var product=await _context.Product.Where(x=>x.Id==item.ProductId).SingleOrDefaultAsync();
                        var skuProductQty = await _context.ProductSku.Where(x => x.ProductId == item.ProductId && x.SkuId == item.SkuId).FirstOrDefaultAsync();
                        if(!product.IsActive)
                        {
                             var issue=new ProductIssues(){
                                    ProductId=item.ProductId,
                                    ProductName=isZawgyi?Rabbit.Uni2Zg(product.Name):product.Name,
                                    Action="Delete",
                                    Qty=skuProductQty.Qty,
                                    Reason=string.Format("Your order item - {0} has been deleted by seller.",product.Name)
                                };
                                issueList.Add(issue);
                        }
                        
                        else if (skuProductQty != null)
                        {
                            if(item.Qty>skuProductQty.Qty){//Check if add to cart qty > stock qty. Can't make order
                               
                                var issue=new ProductIssues(){
                                    ProductId=item.ProductId,
                                    ProductName=isZawgyi?Rabbit.Uni2Zg(product.Name):product.Name,
                                     Action="OutOfStock",
                                    Qty=skuProductQty.Qty,
                                    Reason=string.Format("You'er order {0} of {1}, but we only have {2} left.",(item.Qty>1?item.Qty+" quantities" : item.Qty+" quantity" ),(product.Name),(skuProductQty.Qty>1?skuProductQty.Qty+" quantities" : skuProductQty.Qty+" quantity" ))
                                };
                                issueList.Add(issue);                                
                            }            
                        }                         
                    }
                    if(issueList.Count()>0)
                    {
                        response.OrderId = 0;
                        response.StatusCode=StatusCodes.Status400BadRequest;
                        response.ProductIssues=issueList;
                        return response;
                    }  

                    #endregion
                                        
                    #region Save Order

                    var voucherNo = await _context.Order
                                    .OrderByDescending(o => o.Id)
                                    .Select(x => x.VoucherNo)
                                    .FirstOrDefaultAsync();
                    if(voucherNo==null){
                            voucherNo="0";
                    }
                    voucherNo = (int.Parse(voucherNo) + 1).ToString();
                    voucherNo = voucherNo.PadLeft(9, '0');
                        
                    var orderToAdd = new Order
                    {
                        OrderDate = DateTime.Now,
                        VoucherNo = voucherNo,
                        TotalAmt = request.TotalAmt,
                        NetAmt=request.NetAmt,
                        DeliveryFee=request.DeliveryFee,
                        TotalPoint=request.TotalPoint,                        
                        CreatedDate = DateTime.Now,
                        CreatedBy = currentUserLogin,
                        OrderStatusId = MayMayShopConst.ORDER_STATUS_ORDER, // orderd
                        OrderUserId = currentUserLogin,
                    };             

                    await _context.Order.AddAsync(orderToAdd);
                    await _context.SaveChangesAsync();

                    foreach (var item in request.ProductInfo)
                    {  
                        var orderDetailToAdd = new OrderDetail
                        {
                            OrderId = orderToAdd.Id,
                            ProductId = item.ProductId,
                            SkuId = item.SkuId,
                            Qty = item.Qty,
                            Price = item.RewardAmount,
                            Point=item.Point,
                            RewardPercent=item.RewardPercent,
                        };
                        var skuProductQty = await _context.ProductSku.Where(x => x.ProductId == item.ProductId && x.SkuId == item.SkuId).FirstOrDefaultAsync();
                        if (skuProductQty != null)
                        {
                            skuProductQty.Qty = skuProductQty.Qty - item.Qty;
                            await _context.SaveChangesAsync();
                        }
                        await _context.OrderDetail.AddAsync(orderDetailToAdd);
                    }
                    
                    #endregion
                
                    #region Delivery Info
                    var orderDeliveryInfoToAdd = new OrderDeliveryInfo
                    {
                        OrderId = orderToAdd.Id,
                        Name =isZawgyi?Rabbit.Zg2Uni(request.DeliveryInfo.Name):request.DeliveryInfo.Name,
                        DeliveryServiceId = request.DeliveryInfo.DeliverServiceId,
                        Address =isZawgyi?Rabbit.Zg2Uni(request.DeliveryInfo.Address):request.DeliveryInfo.Address,
                        PhNo = request.DeliveryInfo.PhoNo,
                        Remark =isZawgyi?Rabbit.Zg2Uni(request.DeliveryInfo.Remark):request.DeliveryInfo.Remark,
                        CityId = request.DeliveryInfo.CityId,
                        TownshipId = request.DeliveryInfo.TownshipId,
                        DeliveryDate = request.DeliveryInfo.DeliveryDate,
                        FromTime=request.DeliveryInfo.FromTime,
                        ToTime=request.DeliveryInfo.ToTime
                    };
                    await _context.OrderDeliveryInfo.AddAsync(orderDeliveryInfoToAdd);
                    #endregion

                    #region Payment Info

                    if (request.PaymentInfo != null)
                    {
                         var path = "";
                        if (!String.IsNullOrEmpty(request.PaymentInfo.ApprovalImage.ApprovalImage))
                        {
                            var res =(await _services.UploadToS3(request.PaymentInfo.ApprovalImage.ApprovalImage
                            , request.PaymentInfo.ApprovalImage.ApprovalImageExtension, "order"));   
                            path = res.ImgPath;
                        }

                        var orderPaymentInfoToAdd = new OrderPaymentInfo
                        {
                            OrderId = orderToAdd.Id,
                            PaymentServiceId = request.PaymentInfo.PaymentServiceId,
                            TransactionDate = DateTime.Now,
                            PhoneNo = request.PaymentInfo.PhoNo,
                            Remark =isZawgyi?Rabbit.Zg2Uni(request.PaymentInfo.Remark):request.PaymentInfo.Remark,
                            ApprovalImgUrl = path,
                            PaymentStatusId = 0
                        };

                        // if payment is bank
                        if(request.PaymentInfo.PaymentServiceId==MayMayShopConst.PAYMENT_SERVICE_BANK)  //if pay by bank, we will add bankID in payment service info
                        {
                            orderPaymentInfoToAdd.BankId=request.PaymentInfo.BankId;
                            orderPaymentInfoToAdd.PaymentStatusId=MayMayShopConst.PAYMENT_STATUS_CHECK;
                        }

                        //if payment is COD
                        else if (request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_COD)
                        {
                            orderPaymentInfoToAdd.PaymentStatusId=MayMayShopConst.PAYMENT_STATUS_SUCCESS;                            
                        }
                        
                        //if payment is PaymentGate way
                        else if (request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_WAVE_MONEY // Wave Money
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_KPAY // KBZPay
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_OK_DOLLAR // OK Dollar
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_MASTER // Master
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_VISA // Visa
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_MYTEL_PAY // My Tel
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_SAISAI_PAY // Sai Sai
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_CB_PAY // CB
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_AYA_PAY // AYA
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_ONE_PAY // ONE Pay                        
                        )
                        {
                            orderPaymentInfoToAdd.PaymentStatusId=MayMayShopConst.PAYMENT_STATUS_SUCCESS;                            
                        }

                        //if payment is PaySlip
                        else if(request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_WAVE_MONEY_MANUAL // Wave Money
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_KPAY_MANUAL // KBZPay
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_OK_DOLLAR_MANUAL // OK Dollar
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_MASTER_MANUAL // Master
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_VISA_MANUAL // Visa
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_MYTEL_PAY_MANUAL // My Tel
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_SAISAI_PAY_MANUAL // Sai Sai
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_CB_PAY_MANUAL // CB
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_AYA_PAY_MANUAL // AYA
                        || request.PaymentInfo.PaymentServiceId == MayMayShopConst.PAYMENT_SERVICE_ONE_PAY_MANUAL // ONE Pay
                        )
                        {
                            orderPaymentInfoToAdd.PaymentStatusId=MayMayShopConst.PAYMENT_STATUS_CHECK;
                        }

                        await _context.OrderPaymentInfo.AddAsync(orderPaymentInfoToAdd);                       
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region MemberPoint

                    var productName =await _context.Product
                                    .Where(x=>x.Id==request.ProductInfo.FirstOrDefault().ProductId)
                                    .Select(x=>x.Name).SingleOrDefaultAsync();

                    var req=new RedemptionMemberPointRequest(){
                        UserId=currentUserLogin,
                        Point=request.TotalPoint,
                        ApplicationConfigId=MayMayShopConst.APPLICATION_CONFIG_ID,
                        ProductName=productName,
                    };

                    await _memberPointServices.RedemptionMemberPoint(req,token);

                    #endregion

                    #region  Noti
                     var sellerList = await _userServices.GetAllSellerUserId(token); 
                        foreach(var seller in sellerList)
                        {                            
                            NotificationTemplate notiTemplate = await _context.NotificationTemplate.
                                Where(a => a.ActionName == "Order").SingleOrDefaultAsync();
                            var body = notiTemplate.Body.Replace("{userName}", orderDeliveryInfoToAdd.Name);
                            
                            Models.Notification notification = new Models.Notification();
                            notification.Title = notiTemplate.Title;
                            notification.Body = currentUserLogin.ToString() + " မှ အော်ဒါမှာယူခဲ့သည်";
                            notification.UserId = seller.Id; //userId;
                            notification.ImgUrl = MayMayShopConst.AWS_USER_PROFILE_PATH + currentUserLogin + ".png";
                            notification.RedirectAction =MayMayShopConst.NOTI_REDIRECT_ACTION_ORDER_DETAIL;
                            notification.ReferenceAttribute = orderToAdd.Id.ToString();
                            notification.CreatedDate = DateTime.Now;
                            notification.CreatedBy = 1;
                            await _context.Notification.AddAsync(notification);
                            await _context.SaveChangesAsync();                            
                            var test = Helpers.Notification.SendFCMNotification(seller.Id.ToString(),
                                                                            notiTemplate.Title,
                                                                            body,
                                                                            seller.Id,
                                                                            MayMayShopConst.NOTI_REDIRECT_ACTION_ORDER_DETAIL,
                                                                            orderToAdd.Id,
                                                                            notification.Id,true);//true for send noti to seller

                            }
                    await transaction.CommitAsync();    
                        
                    #endregion
                       
                    response.OrderId = orderToAdd.Id;
                    response.StatusCode = StatusCodes.Status200OK;
                    return response;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return null;
                }           
            }      
        }      
        public async Task<GetCartDetailForRewardResponse> GetCartDetailForReward(int productId,int skuId,int userId, string token)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            var response = new GetCartDetailForRewardResponse(){
                StatusCode=StatusCodes.Status200OK
            };

            #region  Product info

            var productSku= await _context.ProductSku
                            .Where(x=>x.ProductId==productId
                            && x.SkuId==skuId).SingleOrDefaultAsync();

             var itemForRedeem = await _context.ProductReward.Where(x => x.ProductId == productId
                                && x.StartDate.Date<=DateTime.Now.Date
                                && x.EndDate.Date>=DateTime.Now.Date)
                                .Select(s => new GetCartDetailForRewardProductInfo
                                {
                                    ProductId = s.ProductId,
                                    SkuId = skuId,
                                    OriginalPrice = _context.ProductPrice
                                                    .Where(x => x.ProductId == s.ProductId)
                                                    .Select(s => s.Price)
                                                    .FirstOrDefault(),
                                    ProductUrl = _context.ProductImage
                                                .Where(x => x.ProductId == s.ProductId && x.isMain == true)
                                                .Select(s => s.Url).FirstOrDefault(),
                                    Qty = 1,
                                    Name = _context.Product
                                            .Where(x => x.Id == s.ProductId)
                                            .Select(s => isZawgyi?Rabbit.Uni2Zg(s.Name):s.Name)
                                            .FirstOrDefault(),
                                    Variation = null,
                                    AvailableQty = _context.ProductSku
                                                .Where(x => x.ProductId == s.ProductId 
                                                && x.SkuId == skuId)
                                                .Select(s => s.Qty)
                                                .FirstOrDefault(),
                                    RewardAmount=s.RewardAmount,
                                    RewardPercent=s.RewardPercent,
                                    FixedPrice=s.FixedAmount,
                                    Point=s.Point      
                                }).SingleOrDefaultAsync();

            if(itemForRedeem!=null)
            {      
                if(itemForRedeem.FixedPrice>0)
                {
                    itemForRedeem.RewardAmount=productSku.Price-itemForRedeem.FixedPrice;                   
                }
                else if (itemForRedeem.RewardPercent>0){
                    double discountPrice= ((float)((float)itemForRedeem.RewardPercent / (float)100)) * (float)productSku.Price ;
                    itemForRedeem.RewardAmount=productSku.Price-discountPrice;
                }
                else{
                    itemForRedeem.RewardAmount=0;
                    itemForRedeem.RewardPercent=0;
                    itemForRedeem.FixedPrice=0;
                }     
            }

            var skuValue = await (from psku in _context.ProductSkuValue
                                      from pvopt in _context.ProductVariantOption
                                      where psku.ProductId == itemForRedeem.ProductId
                                      && psku.SkuId == itemForRedeem.SkuId
                                      && psku.ProductId == pvopt.ProductId
                                      && psku.VariantId == pvopt.VariantId
                                      && psku.ValueId == pvopt.ValueId
                                      select isZawgyi?Rabbit.Uni2Zg(pvopt.ValueName):pvopt.ValueName).ToListAsync();

            itemForRedeem.Variation = string.Join(",", skuValue);

            var productInfo=new List<GetCartDetailForRewardProductInfo>();
            productInfo.Add(itemForRedeem);

            response.ProductInfo =productInfo;

            #endregion
           
            #region  Payment info
             response.PaymentService = await _context.PaymentService.Where(x => x.Id != MayMayShopConst.PAYMENT_SERVICE_COD && x.IsActive == true)
                                    .Select(s => new GetCartDetailForRewardPaymentService
                                    {
                                        Id = s.Id,
                                        ImgUrl = s.ImgPath,
                                        Name = isZawgyi?Rabbit.Uni2Zg(s.Name):s.Name
                                    }).ToListAsync();                    
            response.NewPaymentService=await _miscellaneousRepo.GetPaymentServiceForBuyer();
            #endregion

            #region  Delivery info

            var userInfo = await _userServices.GetUserInfo(userId, token);
            
            var trnCartDeliInfo = await _context.TrnCartDeliveryInfo.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (trnCartDeliInfo != null)
            {
                string cityName=await _deliServices.GetCityName(token,trnCartDeliInfo.CityId);
                string townshipName=await _deliServices.GetTownshipName(token,trnCartDeliInfo.TownshipId);

                if(userInfo.UpdatedDate != null && userInfo.UpdatedDate > trnCartDeliInfo.UpdatedDate)
                {
                    userInfo.Address=isZawgyi?Rabbit.Uni2Zg(userInfo.Address):userInfo.Address;
                    trnCartDeliInfo.Name =isZawgyi?Rabbit.Uni2Zg( userInfo.Name):userInfo.Name;
                    trnCartDeliInfo.Address = userInfo.Address==null?" ":userInfo.Address;
                    trnCartDeliInfo.PhNo = userInfo.PhoneNo;
                    trnCartDeliInfo.TownshipId = userInfo.TownshipId;
                    trnCartDeliInfo.CityId = userInfo.CityId;
                    await _context.SaveChangesAsync();
                }

                GetCartDetailForRewardDeliveryInfo cartDetailDeliveryInfo = new GetCartDetailForRewardDeliveryInfo();
                cartDetailDeliveryInfo.UserId = trnCartDeliInfo.UserId;
                cartDetailDeliveryInfo.CityId = trnCartDeliInfo.CityId;
                cartDetailDeliveryInfo.TownshipId = trnCartDeliInfo.TownshipId;
                cartDetailDeliveryInfo.AreaInfo = townshipName + " ၊ " +cityName;
                cartDetailDeliveryInfo.Address = trnCartDeliInfo.Address==null?" ":trnCartDeliInfo.Address;
                cartDetailDeliveryInfo.DeliveryAmt = trnCartDeliInfo.DeliveryAmt;
                cartDetailDeliveryInfo.DeliveryServiceId = trnCartDeliInfo.DeliveryServiceId;
                cartDetailDeliveryInfo.FromEstDeliveryDay = trnCartDeliInfo.FromEstDeliveryDay;
                cartDetailDeliveryInfo.ToEstDeliveryDay = trnCartDeliInfo.ToEstDeliveryDay;
                cartDetailDeliveryInfo.CityName = await _deliServices.GetCityName(token,trnCartDeliInfo.CityId);
                cartDetailDeliveryInfo.TownshipName = await _deliServices.GetTownshipName(token,trnCartDeliInfo.TownshipId);
                cartDetailDeliveryInfo.Name = trnCartDeliInfo.Name;
                cartDetailDeliveryInfo.PhoNo = trnCartDeliInfo.PhNo;
                cartDetailDeliveryInfo.Remark = trnCartDeliInfo.Remark;
                if (trnCartDeliInfo.DeliveryDate != DateTime.MinValue)
                {
                    cartDetailDeliveryInfo.DeliveryDate = trnCartDeliInfo.DeliveryDate.Date.ToString("dd MMM yyyy") + "(" + trnCartDeliInfo.DeliveryFromTime + " - " + trnCartDeliInfo.DeliveryToTime + ")";
                }
                else
                {
                    cartDetailDeliveryInfo.DeliveryDate = String.Empty;
                }
                cartDetailDeliveryInfo.DeliveryFromTime = trnCartDeliInfo.DeliveryFromTime;
                cartDetailDeliveryInfo.DeliveryToTime = trnCartDeliInfo.DeliveryToTime;
                response.DeliveryInfo = cartDetailDeliveryInfo;
               
            }
            else
            {

                string cityName=await _deliServices.GetCityName(token,userInfo.CityId);
                string townshipName=await _deliServices.GetTownshipName(token,userInfo.TownshipId);

                cityName=isZawgyi?Rabbit.Uni2Zg(cityName):cityName;
                townshipName=isZawgyi?Rabbit.Uni2Zg(townshipName):townshipName;

                userInfo.Address=isZawgyi?Rabbit.Uni2Zg(userInfo.Address):userInfo.Address;

                #region GetDeliveryServiceRate
                var deliveryRate= await _deliServices.GetDeliveryServiceRate(MayMayShopConst.CUSTOM_DELIVERY_SERVICE_ID,
                                int.Parse(userInfo.CityId.ToString()),
                                int.Parse(userInfo.TownshipId.ToString()),
                                token);               
                #endregion

                var deliveryInfo= new GetCartDetailForRewardDeliveryInfo()
                {
                    CityId = userInfo.CityId,
                    TownshipId = userInfo.TownshipId,
                    AreaInfo =  townshipName + " ၊ " +cityName,
                    CityName =  cityName,
                    TownshipName =  townshipName,
                    Address = String.IsNullOrEmpty(userInfo.Address)?" ":userInfo.Address,
                    DeliveryAmt = deliveryRate.ServiceAmount,
                    DeliveryServiceId = MayMayShopConst.CUSTOM_DELIVERY_SERVICE_ID,
                    FromEstDeliveryDay = deliveryRate.FromEstDeliveryDay,
                    ToEstDeliveryDay = deliveryRate.ToEstDeliveryDay,
                    UserId = userId,
                    Name = isZawgyi?Rabbit.Uni2Zg(userInfo.Name):userInfo.Name,
                    PhoNo = userInfo.PhoneNo,
                    Remark = String.Empty,
                    DeliveryDate = DateTime.Now.Date.AddDays(deliveryRate.ToEstDeliveryDay).ToString("dd MMM yyyy") + "(3PM - 5PM)",
                    DeliveryFromTime = String.Empty,
                    DeliveryToTime = String.Empty,
                };
                response.DeliveryInfo = deliveryInfo;
                }


            #endregion
            
            #region  Total amount info
            
            response.TotalAmt += response.ProductInfo.FirstOrDefault().RewardAmount * response.ProductInfo.FirstOrDefault().Qty;
            response.TotalPoint+=response.ProductInfo.FirstOrDefault().Point;  

            response.DeliveryFee=response.DeliveryInfo.DeliveryAmt;
            response.NetAmt = response.TotalAmt + response.DeliveryInfo.DeliveryAmt;

            var req=new GetMyOwnPointRequest(){
                UserId=userId
            };
            var myOwnPoint=await _memberPointServices.GetMyOwnPoint(req,token);
            response.MyOwnPoint=myOwnPoint.TotalPoint;
            #endregion

            #region Check qty before order

            var issueList=new List<ProductIssues>();
            foreach (var item in response.ProductInfo)
            {
                var product=await _context.Product.Where(x=>x.Id==item.ProductId).SingleOrDefaultAsync();
                var skuProductQty = await _context.ProductSku.Where(x => x.ProductId == item.ProductId && x.SkuId == item.SkuId).FirstOrDefaultAsync();
                if(!product.IsActive)
                {
                        var issue=new ProductIssues(){
                            ProductId=item.ProductId,
                            SkuId=item.SkuId,
                            ProductName=isZawgyi?Rabbit.Uni2Zg(product.Name):product.Name,
                            Action="Delete",
                            Qty=skuProductQty.Qty,
                            Reason=string.Format("Your order item - {0} has been deleted by seller.",product.Name)
                        };
                        issueList.Add(issue);
                }
                
                else if (skuProductQty != null)
                {
                    if(item.Qty>skuProductQty.Qty){//Check if add to cart qty > stock qty. Can't make order
                        
                        var issue=new ProductIssues(){
                            ProductId=item.ProductId,
                            ProductName=isZawgyi?Rabbit.Uni2Zg(product.Name):product.Name,
                                Action="OutOfStock",
                            Qty=skuProductQty.Qty,
                            Reason=string.Format("You'er order {0} of {1}, but we only have {2} left.",(item.Qty>1?item.Qty+" quantities" : item.Qty+" quantity" ),(product.Name),(skuProductQty.Qty>1?skuProductQty.Qty+" quantities" : skuProductQty.Qty+" quantity" ))
                        };
                        issueList.Add(issue);                                
                    }            
                }                         
            }
            if(issueList.Count()>0)
            {              
                response.ProductIssues=issueList;
            }  
            #endregion

            return response;
           
        }
        public async Task<PostOrderByKBZPayResponse> RedeemOrderByKBZPay(RedeemOrderRequest req, int userId, string token)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            #region Check qty before order

             PostOrderByKBZPayResponse response = new PostOrderByKBZPayResponse(){
                        OrderId=0,
                        Timestamp=0,
                        NonceStr="",
                        TransactionId="",
                        Precreate=new KBZPrecreateResponse(){
                            Response=new Response(){
                                result="",
                                code="",
                                msg="",
                                merch_order_id="",
                                prepay_id="",
                                nonce_str="",
                                sign_type="",
                                sign=""
                            }
                        }
                    };


                    var issueList=new List<ProductIssues>();
                    
                    foreach (var item in req.ProductInfo)
                    {
                        var product=await _context.Product.Where(x=>x.Id==item.ProductId).SingleOrDefaultAsync();
                        var skuProductQty = await _context.ProductSku.Where(x => x.ProductId == item.ProductId && x.SkuId == item.SkuId).FirstOrDefaultAsync();
                        if(!product.IsActive)
                        {
                             var issue=new ProductIssues(){
                                    ProductId=item.ProductId,
                                    ProductName=isZawgyi?Rabbit.Uni2Zg(product.Name):product.Name,
                                    Action="Delete",
                                    Qty=skuProductQty.Qty,
                                    Reason=string.Format("Your order item - {0} has been deleted by seller.",product.Name)
                                };
                                issueList.Add(issue);
                        }
                        
                        else if (skuProductQty != null)
                        {
                            if(item.Qty>skuProductQty.Qty){//Check if add to cart qty > stock qty. Can't make order
                               
                                var issue=new ProductIssues(){
                                    ProductId=item.ProductId,
                                    ProductName=isZawgyi?Rabbit.Uni2Zg(product.Name):product.Name,
                                     Action="OutOfStock",
                                    Qty=skuProductQty.Qty,
                                    Reason=string.Format("You'er order {0} of {1}, but we only have {2} left.",(item.Qty>1?item.Qty+" quantities" : item.Qty+" quantity" ),(product.Name),(skuProductQty.Qty>1?skuProductQty.Qty+" quantities" : skuProductQty.Qty+" quantity" ))
                                };
                                issueList.Add(issue);                                
                            }            
                        }                         
                    }
                    if(issueList.Count()>0)
                    {
                        response.OrderId = 0;
                        response.StatusCode=StatusCodes.Status400BadRequest;
                        response.ProductIssues=issueList;
                        return response;
                    }  

                    #endregion
                 

            var transactionID= System.Guid.NewGuid().ToString()+MayMayShopConst.APPLICATION_CONFIG_ID;
            OrderTransaction transaction = new OrderTransaction(){
                Id= transactionID,
                TransactionData = JsonConvert.SerializeObject(req),
                CreatedBy = userId,
                CreatedDate = DateTime.Now
            };
            _context.OrderTransaction.Add(transaction);
            await _context.SaveChangesAsync();

            response = await _paymentservices.KBZPrecreate(transactionID,req.TotalAmt,req.PlatForm);
            response.TransactionId = transaction.Id;
            return response;
        }
        public async Task<List<GetConfigMemberPointProductCategory>> GetProductCategoryForCreateConfigMemberPoint(string token)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            var productCategoryList=await _memberPointServices.GetProductCategoryForCreateConfigMemberPoint(token);
                        
            var productCategoryId=productCategoryList.Select(x=>x.ProductCategoryId).ToArray();
            
            List<int?> mCID=await _context.ProductCategory
                .Where(x=>x.IsDeleted!=true && x.SubCategoryId==null)
                .Select(x=>x.Id)
                .Distinct()
                .Cast<int?>()
                .ToListAsync();

            var productCategory= await _context.ProductCategory
                    .Where(x=>x.IsDeleted!=true
                    && !productCategoryId.Contains(x.Id)
                    && mCID.Contains(x.SubCategoryId)
                    && x.SubCategoryId!=null 
                    && x.SubCategoryId!=0)
                    .Select(x=> new GetConfigMemberPointProductCategory{
                        ProductCategoryId=x.Id,
                        ProductCategoryName=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                        Url=x.Url,
                        ConfigMemberPointId=0,
                        ApplicationConfigId=MayMayShopConst.APPLICATION_CONFIG_ID
                    }).ToListAsync();               
                            
            return productCategory;
        }
        public async Task<GetOrderDetailForMemberPoint_MS_Response> GetOrderDetailForMemberPoint_MS(string voucherNo)
        {   
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            var data=await _context.Order
            .Where(x=>x.VoucherNo==voucherNo)
            .Select(x=>new GetOrderDetailForMemberPoint_MS_Response{
                OrderId=x.Id,
                VoucherNo=x.VoucherNo,
                OrderDate=x.OrderDate,
                OrderStatus=_context.OrderStatus.Where(o=>o.Id==x.OrderStatusId).Select(o=>o.Name).FirstOrDefault(),
                TotalAmount=x.TotalAmt,
                DeliveryFee=x.DeliveryFee,
                NetAmount=x.NetAmt
            }).FirstOrDefaultAsync();

            var orderDetail=await _context.OrderDetail
            .Where(x=>x.OrderId==data.OrderId)
            .ToListAsync();

            data.ProductUrl=await _context.ProductImage
            .Where(x=>x.ProductId==orderDetail.FirstOrDefault().ProductId
            && x.isMain==true)
            .Select(x=>x.Url)
            .FirstOrDefaultAsync();

            data.TotalItem=orderDetail.Count();

            var paymentStatusId=await _context.OrderPaymentInfo
            .Where(x=>x.OrderId==data.OrderId)
            .OrderByDescending(x=>x.TransactionDate)
            .Select(x=>x.PaymentStatusId)
            .FirstOrDefaultAsync();

            data.PaymentStatus=await _context.PaymentStatus.Where(x=>x.Id==paymentStatusId).Select(x=>isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name).FirstOrDefaultAsync();
            return data;
        }
        public async Task<List<GetProductListForAddProductRewardResponse>> GetProductListForAddProductReward(GetProductListForAddProductRewardRequest request)
        {
            //product id that qty is 0 or less than 0.
            var productSkuIDs=await (from sku in _context.ProductSku
                                group sku by sku.ProductId into newSku
                                select new
                                {
                                ProductId = newSku.Key,
                                TotalQty = newSku.Sum(x => x.Qty), 
                                })
                                .Where(x=>x.TotalQty<=0)
                                .Select(x=>x.ProductId)
                                .ToArrayAsync();

            var stillRewardProductIDs=await _context.ProductReward
            .Where(x=>x.EndDate.Date>=DateTime.Now.Date)
            .Select(x=>x.ProductId)
            .ToListAsync();

            var promotionProductIDs= await _context.ProductPromotion
            .Where(x=>x.Percent>0)
            .Select(x=>x.ProductId)
            .ToListAsync();

            return await _context.Product
            .Where(x=>x.IsActive==true &&
            (stillRewardProductIDs.Count()==0 || !stillRewardProductIDs.Contains(x.Id))
            && (promotionProductIDs.Count()==0|| !promotionProductIDs.Contains(x.Id))
            && (string.IsNullOrEmpty(request.SearchText) ||x.Name.Contains(request.SearchText))
            && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(x.Id))
            )
            .Select(x=>new GetProductListForAddProductRewardResponse{
                ProductId=x.Id,
                Name=x.Name,
                Qty=_context.ProductSku.Where(sku=>sku.ProductId==x.Id).Sum(sku=>sku.Qty),
                OriginalPrice=_context.ProductPrice.Where(p=>p.ProductId==x.Id).Select(p=>p.Price).FirstOrDefault(),
                Url=_context.ProductImage.Where(i=>i.ProductId==x.Id && i.isMain==true).Select(i=>i.Url).FirstOrDefault()
            })
            .Skip((request.PageNumber-1)*request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        }
    }
}
