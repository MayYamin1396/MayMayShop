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
using MayMayShop.API.Dtos;
using System;
using Microsoft.AspNetCore.Http;
using log4net;
using MayMayShop.API.Const;
using MayMayShop.API.Interfaces.Services;
using MayMayShop.API.Helpers;
using MayMayShop.API.Dtos.MiscellanceousDto;

namespace MayMayShop.API.Repos
{
    public class MiscellaneousRepository : IMiscellaneousRepository
    {
        private readonly MayMayShopContext _context;
        private readonly IMayMayShopServices _services;        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MiscellaneousRepository(MayMayShopContext context,IMayMayShopServices service,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _services=service;
            _httpContextAccessor=httpContextAccessor;
        }
        public Image FixedSize(Image imgPhoto, int width, int height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)width / (float)sourceWidth);
            nPercentH = ((float)height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
           

            Bitmap bmPhoto = new Bitmap(width, height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Gray);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
        public async Task<List<GetMainCategoryResponse>> GetMainCategory()
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);
           
            return await _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true && (x.SubCategoryId==0 || string.IsNullOrEmpty(x.SubCategoryId.ToString())))
                        .Select(x=> new GetMainCategoryResponse
                        {Id=x.Id,
                        Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                        Description=isZawgyi?Rabbit.Uni2Zg(x.Description):x.Description,
                        Url=x.Url,
                        SubCategory= _context.ProductCategory
                        .Where(a=>a.IsDeleted!=true && a.SubCategoryId==x.Id)
                        .Select(a=> new GetSubCategoryResponse{Id=a.Id,
                                                               Name=isZawgyi?Rabbit.Uni2Zg(a.Name):a.Name,
                                                               Description=isZawgyi?Rabbit.Uni2Zg(a.Description):a.Description,
                                                               Url=a.Url,
                                                               MainCategoryId=x.Id
                                                               })
                        .ToList()
                        })
                        .ToListAsync();
        }
        public async Task<List<GetSubCategoryResponse>> GetSubCategory(GetSubCategoryRequest request)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);
           
             var mainCategoryId=int.Parse(request.MainCategoryId.ToString());
             return await _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true && x.SubCategoryId==mainCategoryId)
                        .Select(x=> new GetSubCategoryResponse{Id=x.Id,
                        Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                        Description=isZawgyi?Rabbit.Uni2Zg(x.Description):x.Description,
                        Url=x.Url,
                        MainCategoryId=int.Parse(x.SubCategoryId.ToString())})
                        .ToListAsync();
        }
        public async Task<List<SearchTagResponse>> SearchTag(SearchTagRequest request)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            if(isZawgyi)
            {
                //convert zawgyi to unicode for search because we only can search unicode in db.
                request.SearchText=Rabbit.Zg2Uni(request.SearchText);
            }
            return await _context.Tag.Where(x=>x.Name.Contains(request.SearchText))
                        .Select(x=>new SearchTagResponse{
                            Id=x.Id,
                            Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name
                            }).ToListAsync();
        }
        public async Task<List<GetBankResponse>> GetBank()
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.Bank
            .Where(x=>x.IsDelete!=true)
            .Select(x=>new GetBankResponse{
                Id=x.Id,
                Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                Url=x.Url,
                SelectUrl=x.SelectUrl,
                AccountNo=x.AccountNo,
                HolderName=isZawgyi?Rabbit.Uni2Zg(x.HolderName):x.HolderName               
            }).OrderBy(x=>x.Id).ToListAsync();
        }
        public async Task<List<GetTagResponse>> GetTag()
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.Tag.Select(x=>new GetTagResponse{
                Id=x.Id,
                Name=isZawgyi? Rabbit.Uni2Zg(x.Name):x.Name,                     
            }).OrderBy(x=>x.Id).ToListAsync();
        }
        public async Task<List<SearchCategoryResponse>> SearchCategory(string searchText)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            if(isZawgyi)
            {
                searchText=Rabbit.Zg2Uni(searchText);
            }
            return await _context.ProductCategory
                    .Where(x=> (string.IsNullOrEmpty(searchText) || x.Name.Contains(searchText))
                    && (x.SubCategoryId==null || x.SubCategoryId==0)
                    && x.IsDeleted!=true)
                    .Select(x=>new SearchCategoryResponse{
                        Id=x.Id,
                        Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                        Url=x.Url,
                        SubCount=_context.ProductCategory
                                .Where(a=>a.SubCategoryId==x.Id
                                && a.IsDeleted!=true)
                                .Count()
                    })
                    .ToListAsync();
        }
        public async Task<List<GetCategoryIconResponse>> GetCategoryIcon()
        {
            return await _context.CategoryIcon
                    .Select(x=>new GetCategoryIconResponse{
                        Url=x.Url
                    }).ToListAsync();
        }
        public async Task<ResponseStatus> CreateMainCategory(CreateMainCategoryRequest request,int currentUserLogin)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

                    if(isZawgyi)
                    {
                        request.Name=Rabbit.Zg2Uni(request.Name);
                    }

                    if(_context.ProductCategory.Any(x=>x.Name==request.Name && x.SubCategoryId == null && x.IsDeleted == false))
                    {
                        return new ResponseStatus(){StatusCode=StatusCodes.Status400BadRequest,Message="Main category name is duplicated!"}; 
                    }
                    ProductCategory mainCate=new ProductCategory()
                    {
                        Name=request.Name,
                        Url=request.Url,
                        VideoUrl=request.VideoUrl,
                        CreatedBy=currentUserLogin,
                        CreatedDate=DateTime.Now
                    };
                    _context.ProductCategory.Add(mainCate);
                    await _context.SaveChangesAsync();

                    if (mainCate.Id != 0)
                    {
                        foreach(var item in request.SubCategory)
                        {
                            item.Name=isZawgyi?Rabbit.Zg2Uni(item.Name):item.Name;
                            if(!_context.ProductCategory.Any(x=>x.Id == mainCate.Id && x.Name==item.Name && x.SubCategoryId != null && x.IsDeleted == false))
                            {
                                
                                ProductCategory subCate=new ProductCategory(){
                                SubCategoryId=mainCate.Id,
                                Name=item.Name,
                                Url=item.Url,
                                CreatedBy=currentUserLogin,
                                CreatedDate=DateTime.Now
                                };
                                _context.ProductCategory.Add(subCate);
                                await _context.SaveChangesAsync();  

                                foreach (var vari in item.VariantList)
                                {
                                    vari.Name=isZawgyi?Rabbit.Zg2Uni(vari.Name):vari.Name;

                                    if(!_context.Variant.Any(x=>x.ProductCategoryId==subCate.Id && x.Name==vari.Name && x.IsDeleted == false))
                                    {
                                        Variant variant=new Variant(){
                                        ProductCategoryId=subCate.Id,
                                        Name=vari.Name,
                                        Description=vari.Name,
                                        CreatedBy=currentUserLogin,
                                        CreatedDate=DateTime.Now
                                        };
                                        _context.Variant.Add(variant);  
                                    }   
                                }
                            }
                        }
                    }

                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Added."};
                }
                catch (Exception e)
                {
                    log.Error(string.Format("error=>{0}, inner exception=>{1}",e.Message,e.InnerException.Message));
                    transaction.Rollback();
                    return new ResponseStatus(){StatusCode = StatusCodes.Status500InternalServerError, Message="Failed"};
                }
            }
        }
        public async Task<ResponseStatus> UpdateMainCategory(UpdateMainCategoryRequest request, int currentUserLogin)
        {
            if(_context.ProductCategory.Any(x=>x.Name==request.Name && x.Id!=request.Id && x.SubCategoryId==null && x.IsDeleted == false))
            {
                return new ResponseStatus(){StatusCode=StatusCodes.Status400BadRequest,Message="Category name is duplicated!"}; 
            }
            
           ProductCategory category=await _context.ProductCategory
                                    .Where(x=>x.Id==request.Id)
                                    .SingleOrDefaultAsync();
            if(category!=null)
            {
                bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);
                 
                request.Name=isZawgyi?Rabbit.Zg2Uni(request.Name):request.Name;

                category.Name=request.Name;
                category.Url=request.Url;
                category.VideoUrl=request.VideoUrl;
                category.UpdatedBy=currentUserLogin;
                category.UpdatedDate=DateTime.Now;
            }
            await _context.SaveChangesAsync();

            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Updated."};
        }
        public async Task<ResponseStatus> DeleteMainCategory(int productCategoryId,int currentUserLogin)
        {
            ProductCategory mainCate=await _context.ProductCategory
                                    .Where(x=>x.Id==productCategoryId)
                                    .SingleOrDefaultAsync();
            if(mainCate!=null)
            {
                var subCate=await _context.ProductCategory
                            .Where(x=>x.SubCategoryId==productCategoryId)
                            .ToListAsync();
                foreach(var item in subCate)
                {
                    item.IsDeleted=true;

                     var variant=await _context.Variant.Where(x=>x.ProductCategoryId==item.Id).ToListAsync();
                    foreach(var v in variant)
                    {
                        v.IsDeleted=true;
                    }
                }
                mainCate.IsDeleted=true;
            }
            await _context.SaveChangesAsync();

             return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Deleted."};
        }
        public async Task<GetMainCategoryByIdResponse> GetMainCategoryById(int productCategoryId)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

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

            return await _context.ProductCategory
                    .Where(x=>x.Id==productCategoryId)
                    .Select(x=>new GetMainCategoryByIdResponse{
                        Id=x.Id,
                        Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                        Url=x.Url,
                        VideoUrl=x.VideoUrl,
                        SubCategory=_context.ProductCategory
                                    .Where(a=>a.SubCategoryId==x.Id
                                    && a.IsDeleted!=true)
                                    .Select(a=>new GetSubCategoryByIdResponse{
                                        Id=a.Id,
                                        MainCategoryId=a.SubCategoryId,
                                        Name=isZawgyi?Rabbit.Uni2Zg(a.Name):a.Name,
                                        Url=a.Url,
                                        ProductCount=_context.Product
                                                    .Where(p=>p.ProductCategoryId==a.Id
                                                    && p.IsActive==true
                                                    &&!productSkuIDs.Contains(p.Id) )
                                                    .Count(),
                                        Variant=_context.Variant
                                                .Where(v=>v.ProductCategoryId==a.Id && v.IsDeleted!=true)
                                                .Select(v=>new GetVariantBySubCategoryResponse{
                                                    SubCategoryId=a.Id,
                                                    VariantId=v.Id,
                                                    VariantName=isZawgyi?Rabbit.Uni2Zg(v.Name):v.Name
                                                }).ToList()
                                    }).ToList()
                    }).SingleOrDefaultAsync();
        }
        public async Task<GetSubCategoryResponse> CreateSubCategory(CreateSubCategoryRequest request, int currentUserLogin)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);
            
            request.Name=isZawgyi?Rabbit.Zg2Uni(request.Name):request.Name;

            if(_context.ProductCategory.Any(x=>x.Name==request.Name && x.SubCategoryId != null && x.IsDeleted == false))
            {
                return new GetSubCategoryResponse(){StatusCode=StatusCodes.Status400BadRequest,Message="Sub category name is duplicated!"}; 
            }    
            ProductCategory category=new ProductCategory(){
                SubCategoryId=request.MainCategoryId,
                Name=request.Name,
                Url=request.Url,
                CreatedDate=DateTime.Now,
                CreatedBy=currentUserLogin
            };
            _context.ProductCategory.Add(category);
            await _context.SaveChangesAsync();

            foreach(var item in request.VariantList)
            {
                item.Name=isZawgyi?Rabbit.Zg2Uni(item.Name):item.Name;

                if(!_context.Variant.Any(x=>x.ProductCategoryId==category.Id && x.Name==item.Name && x.IsDeleted == false))
                {
                Variant variant=new Variant(){
                    ProductCategoryId=category.Id,
                    Name=item.Name,
                    Description=item.Name,
                    CreatedBy=currentUserLogin,
                    CreatedDate=DateTime.Now
                    };  
                    
                _context.Variant.Add(variant);
                }
            }           
            await _context.SaveChangesAsync();

            var resp = await _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true && x.Id == category.Id && x.SubCategoryId == request.MainCategoryId)
                        .Select(x=> new GetSubCategoryResponse{
                            Id=x.Id,
                            Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                            Description=isZawgyi?Rabbit.Uni2Zg(x.Description):x.Description,
                            Url=x.Url,
                            MainCategoryId=int.Parse(x.SubCategoryId.ToString())})
                        .FirstOrDefaultAsync();
            if (resp != null)
            {
                resp.StatusCode = StatusCodes.Status200OK;
                resp.Message = "Successfully Added.";
            }
            return resp;

            // return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Added."};
        }
        public async Task<ResponseStatus> UpdateSubCategory(UpdateSubCategoryRequest request, int currentUserLogin)
        {
            if(_context.ProductCategory.Any(x=>x.Name==request.Name && x.Id!=request.Id && x.SubCategoryId!=null && x.IsDeleted == false))
            {
                return new ResponseStatus(){StatusCode=StatusCodes.Status400BadRequest,Message="Sub category name is duplicated!"}; 
            } 
            ProductCategory category=await _context.ProductCategory
                                    .Where(x=>x.Id==request.Id)
                                    .SingleOrDefaultAsync();
            if(category!=null)
            {
                bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

                request.Name=isZawgyi?Rabbit.Zg2Uni(request.Name):request.Name;

                category.Name=request.Name;
                category.Url=request.Url;
                category.UpdatedBy=currentUserLogin;
                category.UpdatedDate=DateTime.Now;
            }
            await _context.SaveChangesAsync();

            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Updated."};
        }
        public async Task<ResponseStatus> DeleteSubCategory(int productCategoryId, int currentUserLogin)
        {
            var variant=await _context.Variant.Where(x=>x.ProductCategoryId==productCategoryId).ToListAsync();
            foreach(var item in variant)
            {
                item.IsDeleted=true;
            }

            ProductCategory category=await _context.ProductCategory
                                    .Where(x=>x.Id==productCategoryId)
                                    .SingleOrDefaultAsync();
            
            if(category!=null)
            {
               category.IsDeleted=true;
            }
            await _context.SaveChangesAsync();

             return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Deleted."};
        }
        public async Task<GetSubCategoryByIdResponse> GetSubCategoryById(int productCategoryId)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.ProductCategory
                    .Where(x=>x.Id==productCategoryId)
                    .Select(x=>new GetSubCategoryByIdResponse{
                    Id=x.Id,
                    MainCategoryId=x.SubCategoryId,
                    Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                    Url=x.Url,
                    Variant=_context.Variant
                                                .Where(v=>v.ProductCategoryId==x.Id && v.IsDeleted!=true)
                                                .Select(v=>new GetVariantBySubCategoryResponse{
                                                    SubCategoryId=x.Id,
                                                    VariantId=v.Id,
                                                    VariantName=isZawgyi?Rabbit.Uni2Zg(v.Name):v.Name
                                                }).ToList()
                    }).SingleOrDefaultAsync();
        }

        public async Task<CreateVariantResponse> CreateVariant(CreateVariantRequest request, int currentUserLogin)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            request.Name=isZawgyi?Rabbit.Zg2Uni(request.Name):request.Name;

            if(_context.Variant.Any(x=>x.ProductCategoryId==request.SubCategoryId && x.Name==request.Name && x.IsDeleted == false))
            {
                return new CreateVariantResponse(){StatusCode=StatusCodes.Status400BadRequest,Message="Variant is duplicated!"};
            }
            Variant variant=new Variant(){
                Name=request.Name,
                Description=request.Name,
                ProductCategoryId=request.SubCategoryId,
                CreatedDate=DateTime.Now,
                CreatedBy=currentUserLogin
            };
            _context.Variant.Add(variant);
            await _context.SaveChangesAsync();
            var resp = new CreateVariantResponse();
            resp.variantId = variant.Id;
            resp.StatusCode = StatusCodes.Status200OK;
            resp.Message = "Successfully Added.";
            return resp;
        }

        public async Task<ResponseStatus> UpdateVariant(UpdateVariantRequest request, int currentUserLogin)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            request.Name=isZawgyi?Rabbit.Zg2Uni(request.Name):request.Name;

            if(_context.Variant.Any(x=>x.Name==request.Name && x.Id!=request.Id && x.ProductCategoryId==request.ProductCategoryId && x.IsDeleted == false))
            {
                return new ResponseStatus(){StatusCode=StatusCodes.Status400BadRequest,Message="Variant is duplicated!"}; 
            }           
            Variant variant= await _context.Variant.Where(x=>x.Id==request.Id).SingleOrDefaultAsync();
            if(variant!=null)
            {
                variant.Name=request.Name;
                variant.Description=request.Name;
                variant.UpdatedBy=currentUserLogin;
                variant.UpdatedDate=DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Updated."}; 
        }

        public async Task<ResponseStatus> DeleteVariant(int variantId, int currentUserLogin)
        {
            Variant variant= await _context.Variant.Where(x=>x.Id==variantId).SingleOrDefaultAsync();
            if(variant!=null)
            {
                variant.IsDeleted=true;                
                await _context.SaveChangesAsync();
            }
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Deleted."}; 
        }

        public async Task<List<GetPolicyResponse>> GetPolicy()
        {
           bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.Policy
                    .OrderBy(x=>x.SerNo)
                    .Select(x=>new GetPolicyResponse{
                        Id=x.Id,
                        SerNo=x.SerNo,
                        Title=isZawgyi?Rabbit.Uni2Zg(x.Title):x.Title,
                        Description=isZawgyi?Rabbit.Uni2Zg(x.Description):x.Description,
                        CreatedBy=x.CreatedBy,
                        CreatedDate=x.CreatedDate
                    }).ToListAsync();
        }

        public async Task<ResponseStatus> CreateBanner(CreateBannerRequest request, int currentUserLogin,string url)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            request.Name=isZawgyi?Rabbit.Zg2Uni(request.Name):request.Name;

            var data=new Banner(){
                Name=request.Name,
                Url=url,
                BannerLinkId=request.BannerLinkId,
                IsActive=true,
                CreatedBy=currentUserLogin,
                CreatedDate=DateTime.Now,
                BannerType=request.BannerType==1?"Banner":"AD"
            };
            _context.Banner.Add(data);
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Added."};
        }

        public async Task<ResponseStatus> UpdateBanner(UpdateBannerRequest request, int currentUserLogin,ImageUrlResponse image)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            request.Name=isZawgyi?Rabbit.Zg2Uni(request.Name):request.Name;

            var data=await _context.Banner.Where(x=>x.Id==request.Id).SingleOrDefaultAsync();
            data.Name=request.Name;
            data.Url=image.ImgPath;
            data.BannerLinkId=request.BannerLinkId;
            data.UpdatedBy=currentUserLogin;
            data.UpdatedDate=DateTime.Now;
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Updated."};
        }

        public async Task<ResponseStatus> DeleteBanner(int id, int currentUserLogin)
        {
            var data=await _context.Banner.Where(x=>x.Id==id).SingleOrDefaultAsync();           
            data.IsActive=false;
            data.UpdatedBy=currentUserLogin;
            data.UpdatedDate=DateTime.Now;
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Deleted."};
        }

        public async Task<GetBannerResponse> GetBannerById(int id)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.Banner.Where(x=>x.Id==id)
                    .Select(x=>new GetBannerResponse{
                        Id=x.Id,
                        Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                        Url=x.Url,
                        BannerLinkId=x.BannerLinkId
                    }).SingleOrDefaultAsync();
        }

        public async Task<List<GetBannerResponse>> GetBannerList(int bannerType)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            string bannerTypeFilter=bannerType==1?"Banner":"AD";
            
            return await _context.Banner.Where(x=>x.IsActive==true
                    && x.BannerType==bannerTypeFilter)
                    .Select(x=>new GetBannerResponse{
                        Id=x.Id,
                        Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                        Url=x.Url,
                        BannerLinkId=x.BannerLinkId
                    }).ToListAsync();
        }

        public async Task<List<GetBannerLinkResponse>> GetBannerLink()
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.BannerLink.Where(x=>x.IsActive==true)
                    .Select(x=>new GetBannerLinkResponse{
                        Id=x.Id,
                        Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                    }).ToListAsync();
        }
    
        #region Activity Log API
        public async Task<string> GetLastActiveByUserId(int userId){
            var data= await _context.ActivityLog
                                .Where(x=>x.ActivityTypeId==MayMayShopConst.ACTIVITY_TYPE_ACTIVE
                                && x.UserId==userId)
                                .OrderByDescending(x=>x.CreatedDate)
                                .FirstOrDefaultAsync();
            if(data!=null)
            {
                return data.CreatedDate.ToString("yyyy-MM-dd H:mm:ss zzz");
            }
            else{
                return string.Empty;
            }
        } 
        #endregion
        public async Task<ResponseStatus> CreatePolicy(CreatePolicyRequest request, int currentUserLogin)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            request.Title=isZawgyi?Rabbit.Zg2Uni(request.Title):request.Title;
            request.Description = isZawgyi?Rabbit.Zg2Uni(request.Description):request.Description;

            var data=new Policy(){
                Title = request.Title,
                Description = request.Description,
                SerNo = request.SeqNo,
                CreatedBy=currentUserLogin,
                CreatedDate=DateTime.Now
            };
            _context.Policy.Add(data);
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Added."};
        }

        public async Task<ResponseStatus> UpdatePolicy(UpdatePolicyRequest request, int currentUserLogin)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            request.Title=isZawgyi?Rabbit.Zg2Uni(request.Title):request.Title;
            request.Description=isZawgyi?Rabbit.Zg2Uni(request.Description):request.Description;

            var data=await _context.Policy.Where(x=>x.Id==request.Id).SingleOrDefaultAsync();
            data.Description = request.Description;
            data.Title = request.Title;
            data.SerNo = request.SeqNo;
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Updated."};
        }

        public async Task<ResponseStatus> DeletePolicy(int Id)
        {
            var policy = await _context.Policy.Where(x => x.Id == Id).FirstOrDefaultAsync();
            _context.Policy.Remove(policy);
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Deleted."};
        }

        public async Task<GetPolicyResponse> GetPolicyById(int Id)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.Policy
                    .Where(x => x.Id == Id)
                    .Select(x=>new GetPolicyResponse{
                        Id=x.Id,
                        SerNo=x.SerNo,
                        Title=isZawgyi?Rabbit.Uni2Zg(x.Title):x.Title,
                        Description=isZawgyi?Rabbit.Uni2Zg(x.Description):x.Description,
                        CreatedBy=x.CreatedBy,
                        CreatedDate=x.CreatedDate
                    }).FirstOrDefaultAsync();
        }

        #region Payment service
        
        public async Task<List<GetPaymentServiceForSellerResponse>> GetPaymentServiceForSeller()
        {
            return await _context.PaymentService
            .OrderBy(x=>x.SerNo)
            .Select(x=>new GetPaymentServiceForSellerResponse{
                PaymentServiceId=x.Id,
                Name=x.Name,
                Url=x.ImgPath,
                PaymentType=x.PaymentType,
                IsActive=x.IsActive,
                BackgroundUrl=x.BackgroundUrl,
                HolderName=x.HolderName,
                AccountNo=x.AccountNo,
                IsPaymentGateWay=x.IsPaymentGateWay
            })
            .ToListAsync();
        }

        public async Task<GetPaymentServiceForSellerResponse> GetPaymentServiceDetail(int paymentServiceId)
        {
            return await _context.PaymentService
            .Where(x=>x.Id==paymentServiceId)
            .Select(x=>new GetPaymentServiceForSellerResponse{
                PaymentServiceId=x.Id,
                Name=x.Name,
                Url=x.ImgPath,
                PaymentType=x.PaymentType,
                IsActive=x.IsActive,
                BackgroundUrl=x.BackgroundUrl,
                HolderName=x.HolderName,
                AccountNo=x.AccountNo,
                IsPaymentGateWay=x.IsPaymentGateWay
            })
            .SingleOrDefaultAsync();
        }

        public async Task<ResponseStatus> UpdatePaymentService(UpdatePaymentServiceRequest request)
        {
            var paymentService=await _context.PaymentService.Where(x=>x.Id==request.PaymentServiceId).SingleOrDefaultAsync();
            if(paymentService!=null)
            {
                paymentService.Name=request.Name;
                paymentService.IsActive=request.IsActive;
                paymentService.HolderName=request.HolderName;
                paymentService.AccountNo=request.AccountNo;
                paymentService.UpdatedDate=DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully updated."};
        }

        public async Task<List<GetBankResponse>> GetBankListForSeller()
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.Bank
            .Select(x=>new GetBankResponse{
                Id=x.Id,
                Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                Url=x.Url,
                SelectUrl=x.SelectUrl,
                AccountNo=x.AccountNo,
                HolderName=isZawgyi?Rabbit.Uni2Zg(x.HolderName):x.HolderName               
            }).OrderBy(x=>x.Id).ToListAsync();
        
        }

        public async Task<GetBankResponse> GetBankDetail(int id)
        {
            bool isZawgyi=Rabbit.IsZawgyi(_httpContextAccessor);

            return await _context.Bank
            .Where(x=>x.Id==id)
            .Select(x=>new GetBankResponse{
                Id=x.Id,
                Name=isZawgyi?Rabbit.Uni2Zg(x.Name):x.Name,
                Url=x.Url,
                SelectUrl=x.SelectUrl,
                AccountNo=x.AccountNo,
                HolderName=isZawgyi?Rabbit.Uni2Zg(x.HolderName):x.HolderName               
            }).SingleOrDefaultAsync();
        }

        public async Task<ResponseStatus> UpdateBank(UpdateBankRequest request)
        {
            var response=new ResponseStatus();
            var bank=await _context.Bank.Where(x=>x.Id==request.Id).SingleOrDefaultAsync();
            if(bank!=null)
            {
                bank.Name=request.Name;
                bank.AccountNo=request.AccountNo;
                bank.HolderName=request.HolderName;
                await _context.SaveChangesAsync();

                response.StatusCode=StatusCodes.Status200OK;
                response.Message="Successfully updated.";
            }
            else{
                response.StatusCode=StatusCodes.Status400BadRequest;
                response.Message="Bank not found!";
            }
            return response;
        }
        #endregion

        #region Category Icon
       
        public async Task<ResponseStatus> CreateCategoryIcon(CreateCategoryIconRequest request,List<ImageUrlResponse> imgList)
        {
            var oldCate=await _context.CategoryIcon.ToListAsync();
            _context.CategoryIcon.RemoveRange(oldCate);

            foreach(var img in imgList)
            {
                var newCat=new CategoryIcon(){
                    Url=img.ImgPath
                };
                _context.CategoryIcon.Add(newCat);
            }
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully saved."};
        }

       
        #endregion
         public async Task<List<GetPaymentServiceForBuyerResponse>> GetPaymentServiceForBuyer()
        {
            var response=new List<GetPaymentServiceForBuyerResponse>();

            var data =await _context.PaymentService
            .Where(x=>x.IsActive==true)
            .OrderBy(x=>x.SerNo)
            .Select(x=>x.GroupName)
            .Distinct()
            .ToListAsync();

            foreach(var item in data)
            {
                var main=await _context.PaymentService.Where(x=>x.GroupName==item).FirstOrDefaultAsync();
                var res=new GetPaymentServiceForBuyerResponse(){
                    Id=main.Id,
                    Name=item,
                    SerNo=main.SerNo,
                    PaymentType=main.PaymentType,
                    Url=main.ImgPath,
                    PaymentServiceGateWay=_context.PaymentService
                                        .Where(n=>n.GroupName==item && n.IsActive==true)
                                        .OrderBy(n=>n.SerNo)
                                        .Select(n=>new PaymentServiceGateWay(){
                                            Id=n.Id,
                                            Name=n.Name,
                                            Url=n.BackgroundUrl,
                                            IsPaymentGateWay=n.IsPaymentGateWay
                                        })
                                        .ToList()
                };
                response.Add(res);
            }
            return response.OrderBy(x=>x.SerNo).ToList();            
        }

        #region 
        public async Task<List<GetBrandResponse>> GetBrand()
        {
            return await _context.Brand.Where(x => x.IsDeleted == false).Select(x=>new GetBrandResponse{
                Id=x.Id,
                Name=x.Name,
                LogoUrl = x.LogoUrl,
                Url=x.Url,               
            }).OrderBy(x=>x.Id).ToListAsync();
        }

        public async Task<ResponseStatus> AddBrand(AddBrandRequest request, string imageurl, string logourl)
        {
            var data=new Brand(){
                Name = request.Name,
                Url = imageurl,
                LogoUrl = logourl
            };
            _context.Brand.Add(data);
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Added."};
        }

        public async Task<ResponseStatus> UpdateBrand(UpdateBrandRequest request, string imageurl, string logourl)
        {
            var data=await _context.Brand.Where(x=>x.Id==request.Id).SingleOrDefaultAsync();
            data.Name=request.Name;
            data.Url=imageurl;
            data.LogoUrl = logourl;
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Updated."};
        }

        public async Task<ResponseStatus> DeleteBrand(int id)
        {
            var data=await _context.Brand.Where(x=>x.Id==id).SingleOrDefaultAsync();
            data.IsDeleted = true;
            await _context.SaveChangesAsync();
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Deleted."};
        }
        #endregion
        
    }
}
