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

namespace MayMayShop.API.Repos
{
    public class MiscellaneousRepository : IMiscellaneousRepository
    {
        private readonly MayMayShopContext _context;
        public MiscellaneousRepository(MayMayShopContext context)
        {
            _context = context;
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
            return await _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true && (x.SubCategoryId==0 || string.IsNullOrEmpty(x.SubCategoryId.ToString())))
                        .Select(x=> new GetMainCategoryResponse
                        {Id=x.Id,
                        Name=x.Name,
                        Description=x.Description,
                        Url=x.Url,
                        SubCategory= _context.ProductCategory
                        .Where(a=>a.IsDeleted!=true && a.SubCategoryId==x.Id)
                        .Select(a=> new GetSubCategoryResponse{Id=a.Id,
                                                               Name=a.Name,
                                                               Description=a.Description,
                                                               Url=a.Url,
                                                               MainCategoryId=x.Id
                                                               })
                        .ToList()
                        })
                        .ToListAsync();
        }
        public async Task<List<GetSubCategoryResponse>> GetSubCategory(GetSubCategoryRequest request)
        {
             var mainCategoryId=int.Parse(request.MainCategoryId.ToString());
             return await _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true && x.SubCategoryId==mainCategoryId)
                        .Select(x=> new GetSubCategoryResponse{Id=x.Id,Name=x.Name,Description=x.Description,Url=x.Url,MainCategoryId=int.Parse(x.SubCategoryId.ToString())})
                        .ToListAsync();
        }
        public async Task<List<SearchTagResponse>> SearchTag(SearchTagRequest request)
        {
            return await _context.Tag.Where(x=>x.Name.Contains(request.SearchText))
                        .Select(x=>new SearchTagResponse{Id=x.Id,Name=x.Name}).ToListAsync();
        }
        public async Task<List<GetBankResponse>> GetBank()
        {
            return await _context.Bank.Select(x=>new GetBankResponse{
                Id=x.Id,
                Name=x.Name,
                Url=x.Url,
                SelectUrl=x.SelectUrl,
                AccountNo=x.AccountNo,
                HolderName=x.HolderName                
            }).OrderBy(x=>x.Id).ToListAsync();
        }
        public async Task<List<GetTagResponse>> GetTag()
        {
            return await _context.Tag.Select(x=>new GetTagResponse{
                Id=x.Id,
                Name=x.Name,                     
            }).OrderBy(x=>x.Id).ToListAsync();
        }
        public async Task<List<SearchCategoryResponse>> SearchCategory(string searchText)
        {
            return await _context.ProductCategory
                    .Where(x=> (string.IsNullOrEmpty(searchText) || x.Name.Contains(searchText))
                    && (x.SubCategoryId==null || x.SubCategoryId==0)
                    && x.IsDeleted!=true)
                    .Select(x=>new SearchCategoryResponse{
                        Id=x.Id,
                        Name=x.Name,
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
                            if(!_context.ProductCategory.Any(x=>x.Name==request.Name && x.SubCategoryId != null && x.IsDeleted == false))
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
                catch (Exception)
                {
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
            return await _context.ProductCategory
                    .Where(x=>x.Id==productCategoryId)
                    .Select(x=>new GetMainCategoryByIdResponse{
                        Id=x.Id,
                        Name=x.Name,
                        Url=x.Url,
                        VideoUrl=x.VideoUrl,
                        SubCategory=_context.ProductCategory
                                    .Where(a=>a.SubCategoryId==x.Id
                                    && a.IsDeleted!=true)
                                    .Select(a=>new GetSubCategoryByIdResponse{
                                        Id=a.Id,
                                        MainCategoryId=a.SubCategoryId,
                                        Name=a.Name,
                                        Url=a.Url,
                                        ProductCount=_context.Product
                                                    .Where(p=>p.ProductCategoryId==a.Id
                                                    && a.IsDeleted!=true)
                                                    .Count(),
                                        Variant=_context.Variant
                                                .Where(v=>v.ProductCategoryId==a.Id && v.IsDeleted!=true)
                                                .Select(v=>new GetVariantBySubCategoryResponse{
                                                    SubCategoryId=a.Id,
                                                    VariantId=v.Id,
                                                    VariantName=v.Name
                                                }).ToList()
                                    }).ToList()
                    }).SingleOrDefaultAsync();
        }
        public async Task<GetSubCategoryResponse> CreateSubCategory(CreateSubCategoryRequest request, int currentUserLogin)
        {
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
                Variant variant=new Variant(){
                ProductCategoryId=category.Id,
                Name=item.Name,
                Description=item.Name,
                CreatedBy=currentUserLogin,
                CreatedDate=DateTime.Now
                };
            _context.Variant.Add(variant);
            }           
            await _context.SaveChangesAsync();

            var resp = await _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true && x.Id == category.Id && x.SubCategoryId == request.MainCategoryId)
                        .Select(x=> new GetSubCategoryResponse{Id=x.Id,Name=x.Name,Description=x.Description,Url=x.Url,MainCategoryId=int.Parse(x.SubCategoryId.ToString())})
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
            return await _context.ProductCategory
                    .Where(x=>x.Id==productCategoryId)
                    .Select(x=>new GetSubCategoryByIdResponse{
                    Id=x.Id,
                    MainCategoryId=x.SubCategoryId,
                    Name=x.Name,
                    Url=x.Url,
                    Variant=_context.Variant
                                                .Where(v=>v.ProductCategoryId==x.Id && v.IsDeleted!=true)
                                                .Select(v=>new GetVariantBySubCategoryResponse{
                                                    SubCategoryId=x.Id,
                                                    VariantId=v.Id,
                                                    VariantName=v.Name
                                                }).ToList()
                    }).SingleOrDefaultAsync();
        }

        public async Task<ResponseStatus> CreateVariant(CreateVariantRequest request, int currentUserLogin)
        {
            if(_context.Variant.Any(x=>x.ProductCategoryId==request.SubCategoryId && x.Name==request.Name && x.IsDeleted == false))
            {
                return new ResponseStatus(){StatusCode=StatusCodes.Status400BadRequest,Message="Variant is duplicated!"};
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
            return new ResponseStatus(){StatusCode=StatusCodes.Status200OK,Message="Successfully Added."};
        }

        public async Task<ResponseStatus> UpdateVariant(UpdateVariantRequest request, int currentUserLogin)
        {
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
            return await _context.Policy
                    .OrderBy(x=>x.SerNo)
                    .Select(x=>new GetPolicyResponse{
                        Id=x.Id,
                        SerNo=x.SerNo,
                        Title=x.Title,
                        Description=x.Description,
                        CreatedBy=x.CreatedBy,
                        CreatedDate=x.CreatedDate
                    }).ToListAsync();
        }

        public async Task<ResponseStatus> CreateBanner(CreateBannerRequest request, int currentUserLogin,string url)
        {
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
            return await _context.Banner.Where(x=>x.Id==id)
                    .Select(x=>new GetBannerResponse{
                        Id=x.Id,
                        Name=x.Name,
                        Url=x.Url,
                        BannerLinkId=x.BannerLinkId
                    }).SingleOrDefaultAsync();
        }

        public async Task<List<GetBannerResponse>> GetBannerList(int bannerType)
        {
            string bannerTypeFilter=bannerType==1?"Banner":"AD";
            
            return await _context.Banner.Where(x=>x.IsActive==true
                    && x.BannerType==bannerTypeFilter)
                    .Select(x=>new GetBannerResponse{
                        Id=x.Id,
                        Name=x.Name,
                        Url=x.Url,
                        BannerLinkId=x.BannerLinkId
                    }).ToListAsync();
        }

        public async Task<List<GetBannerLinkResponse>> GetBannerLink()
        {
            return await _context.BannerLink.Where(x=>x.IsActive==true)
                    .Select(x=>new GetBannerLinkResponse{
                        Id=x.Id,
                        Name=x.Name,
                    }).ToListAsync();
        }
    }
}
