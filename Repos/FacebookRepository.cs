using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MayMayShop.API.Const;
using MayMayShop.API.Context;
using MayMayShop.API.Dtos.FacebookDto;
using MayMayShop.API.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;

namespace MayMayShop.Repos
{
    public class FacebookRepository : IFacebookRepository
    {
        private readonly MayMayShopContext _context;
        public FacebookRepository(MayMayShopContext context)
        {
            _context = context;           
        }
        public async Task<List<FBGetMainCategoryResponse>> GetMainCategory(FBGetMainCategoryRequest request)
        {
             return await _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true && (x.SubCategoryId==0 || string.IsNullOrEmpty(x.SubCategoryId.ToString())))
                        .Select(x=> new FBGetMainCategoryResponse
                        {Id=x.Id,
                        Name=x.Name,
                        Description=x.Description,
                        Url=x.Url
                        })
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
        }
        public async Task<List<FBGetProductListByMainCategoryResponse>> GetProductListByMainCategory(FBGetProductListByMainCategoryRequest request)
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
           
            var categoryIDs= await _context.ProductCategory.Where(x=>x.SubCategoryId==request.ProductCategoryId).Select(x=>x.Id).ToListAsync();

            return await ( (
                            from p in _context.Product
                            where p.IsActive == true
                            && (categoryIDs.Count()==0 || categoryIDs.Contains(p.ProductCategoryId))
                            && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(p.Id))
                            select new FBGetProductListByMainCategoryResponse{
                                Id = p.Id,
                                Name = p.Name,
                                ProductPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                ProductImage = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                            })
                        )
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
        }
        public async Task<List<FBGetLatestProductListResponse>> GetLatestProductList(FBGetLatestProductListRequest request)
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

             return await ( (
                            from p in _context.Product
                            where p.IsActive == true
                            && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(p.Id))
                            orderby p.CreatedDate descending
                            select new FBGetLatestProductListResponse{
                                Id = p.Id,
                                Name = p.Name,
                                Price = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                            })
                        )
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
        }
        public async Task<List<FBGetPopularProductListResponse>> GetPopularProductList(FBGetPopularProductListRequest request)
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
            
            var fromDate=DateTime.Today.AddDays(-MayMayShopConst.BEST_SELLER_DURATION);
                var toDate=DateTime.Now;

                 List<int> orderListIDs = await _context.Order
                                    .Where(x => x.OrderDate.Date >= fromDate.Date 
                                    && x.OrderDate.Date <= toDate.Date)
                                    .Select(x=>x.Id)
                                    .ToListAsync();
                List<int> productListIDS = await _context.OrderDetail
                                    .Where(x=>orderListIDs.Contains(x.OrderId))
                                    .Select(x=>x.ProductId)
                                    .ToListAsync();              

                return await ((
                                from p in _context.Product
                                .Where(x => x.IsActive == true 
                                && productListIDS.Contains(x.Id)
                                && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(x.Id)))
                               select new FBGetPopularProductListResponse{
                                Id = p.Id,
                                Name = p.Name,
                                Price = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                            })
                        )
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
        }
        public async Task<List<FBGetPromotionProductListResponse>> GetPromotionProductList(FBGetPromotionProductListRequest request)
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

            var proIDs=await _context.ProductPromotion
                                .Where(x=>(productSkuIDs.Count()==0 || !productSkuIDs.Contains(x.ProductId)))                                
                                .Select(x=>x.ProductId)
                                .ToListAsync();

                return await ( (
                                from p in _context.Product
                                where p.IsActive == true
                                && (proIDs.Count()==0 || proIDs.Contains(p.Id))                        
                                select new FBGetPromotionProductListResponse{
                                Id = p.Id,
                                Name = p.Name,
                                OriginalPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                PromotePrice = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.TotalAmt).FirstOrDefault(),
                                PromotePercent = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x =>x.Percent).FirstOrDefault(),
                                Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                            })
                        )
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
        }
    }
}