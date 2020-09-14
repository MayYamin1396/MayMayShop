using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MayMayShop.API.Const;
using MayMayShop.API.Context;
using MayMayShop.API.Controllers;
using MayMayShop.API.Dtos;
using MayMayShop.API.Dtos.ProductDto;
using MayMayShop.API.Interfaces.Repos;
using MayMayShop.API.Interfaces.Services;
using MayMayShop.API.Models;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;

namespace MayMayShop.API.Repos
{
    public class ProductRepository : IProductRepository
    {
        private readonly MayMayShopContext _context;
        private readonly IMapper _mapper;
        private readonly IUserServices _userServices;
        private readonly IMayMayShopServices _MayMayShopServices;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ProductRepository(MayMayShopContext context, IMapper mapper,IUserServices userServices,IMayMayShopServices MayMayShopServices)
        {
            _context = context;
            _mapper = mapper;
            _userServices = userServices;
            _MayMayShopServices=MayMayShopServices;
        }
        public async Task<TrnProduct> CreateDemoProduct(int productCategoryId)
        {
            var productToAdd = new TrnProduct
            {
                Id = Guid.NewGuid(),
                ProductCategoryId = productCategoryId,
                CreatedDate = DateTime.Today
            };

            await _context.AddAsync(productToAdd);

            var productFromRepo = await _context.SaveChangesAsync();

            return productToAdd;
        }
        public async Task<List<PopulateSkuResponse>> PopulateSku(PopulateSkuRequest req, Guid productId)
        {
            if (req.Options.Count == 2)
            {
                var trnProductVariantOptions = new List<TrnProductVariantOption>();
                var trnProductSku = new List<TrnProductSku>();
                var trnProductSkuValues = new List<TrnProductSkuValue>();

                foreach (var item in req.Options)
                {

                    var counter = 1;

                    foreach (var value in item.OptionValue)
                    {

                        trnProductVariantOptions.Add(new TrnProductVariantOption
                        {
                            ProductId = productId,
                            VariantId = item.VariantId,
                            ValueId = counter,
                            ValueName = value
                        });
                        counter += 1;
                    }
                };

                var skuCounter = 1;
                foreach (var item in req.Options[0].OptionValue)
                {
                    var trnProductVariantOption = trnProductVariantOptions.Find(x => x.ValueName == item);

                    foreach (var value in req.Options[1].OptionValue)
                    {
                        trnProductSkuValues.Add(new TrnProductSkuValue
                        {
                            ProductId = productId,
                            SkuId = skuCounter,
                            ValueId = trnProductVariantOption.ValueId,
                            VariantId = trnProductVariantOption.VariantId
                        });

                        var trnProductVariantOptionTwo = trnProductVariantOptions.Find(x => x.ValueName == value);

                        trnProductSkuValues.Add(new TrnProductSkuValue
                        {
                            ProductId = productId,
                            SkuId = skuCounter,
                            ValueId = trnProductVariantOptionTwo.ValueId,
                            VariantId = trnProductVariantOptionTwo.VariantId
                        });

                        skuCounter += 1;
                    }
                }

                trnProductSku =
                    (from row in trnProductSkuValues
                     group row by row.SkuId
                    into g
                     select new TrnProductSku()
                     {
                         ProductId = productId,
                         SkuId = (from res in g select res.SkuId).FirstOrDefault()
                     }).ToList();

                trnProductVariantOptions.ForEach(x => _context.TrnProductVariantOption.AddAsync(x));
                trnProductSku.ForEach(x => _context.TrnProductSku.AddAsync(x));
                trnProductSkuValues.ForEach(x => _context.TrnProductSkuValue.AddAsync(x));

                await _context.SaveChangesAsync();

                var totalSku = trnProductSku.Count;

                var resp = new List<PopulateSkuResponse>();
                for (int i = 1; i <= totalSku; i++)
                {
                    var populateSkuResponse = new PopulateSkuResponse()
                    {
                        ProductId = productId,
                        SkuId = i
                    };

                    var trnProductSkuValue = await _context
                        .TrnProductSkuValue.Where(x => x.ProductId == productId && x.SkuId == i).ToListAsync();

                    var option = "";
                    foreach (var item in trnProductSkuValue)
                    {
                        var trnVariant = await _context.TrnProductVariantOption
                            .Where(x => x.ProductId == productId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                            .FirstOrDefaultAsync();

                        if (String.IsNullOrEmpty(option))
                        {
                            option = trnVariant.ValueName;
                        }
                        else
                        {
                            option = option + ", " + trnVariant.ValueName;
                        }
                    }

                    populateSkuResponse.VariantOptions = option;

                    resp.Add(populateSkuResponse);
                }

                return resp;
            }
            else if (req.Options.Count == 1)
            {
                var trnProductVariantOptions = new List<TrnProductVariantOption>();
                var trnProductSku = new List<TrnProductSku>();
                var trnProductSkuValues = new List<TrnProductSkuValue>();

                foreach (var item in req.Options)
                {

                    var counter = 1;

                    foreach (var value in item.OptionValue)
                    {

                        trnProductVariantOptions.Add(new TrnProductVariantOption
                        {
                            ProductId = productId,
                            VariantId = item.VariantId,
                            ValueId = counter,
                            ValueName = value
                        });
                        counter += 1;
                    }
                };

                var skuCounter = 1;
                foreach (var item in req.Options[0].OptionValue)
                {
                    var trnProductVariantOption = trnProductVariantOptions.Find(x => x.ValueName == item);

                    trnProductSkuValues.Add(new TrnProductSkuValue
                    {
                        ProductId = productId,
                        SkuId = skuCounter,
                        ValueId = trnProductVariantOption.ValueId,
                        VariantId = trnProductVariantOption.VariantId
                    });

                    skuCounter += 1;
                }

                trnProductSku =
                    (from row in trnProductSkuValues
                     group row by row.SkuId
                    into g
                     select new TrnProductSku()
                     {
                         ProductId = productId,
                         SkuId = (from res in g select res.SkuId).FirstOrDefault()
                     }).ToList();

                trnProductVariantOptions.ForEach(x => _context.TrnProductVariantOption.AddAsync(x));
                trnProductSku.ForEach(x => _context.TrnProductSku.AddAsync(x));
                trnProductSkuValues.ForEach(x => _context.TrnProductSkuValue.AddAsync(x));

                await _context.SaveChangesAsync();

                var totalSku = trnProductSku.Count;

                var resp = new List<PopulateSkuResponse>();
                for (int i = 1; i <= totalSku; i++)
                {
                    var populateSkuResponse = new PopulateSkuResponse()
                    {
                        ProductId = productId,
                        SkuId = i
                    };

                    var trnProductSkuValue = await _context
                        .TrnProductSkuValue.Where(x => x.ProductId == productId && x.SkuId == i).ToListAsync();

                    var option = "";
                    foreach (var item in trnProductSkuValue)
                    {
                        var trnVariant = await _context.TrnProductVariantOption
                            .Where(x => x.ProductId == productId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                            .FirstOrDefaultAsync();

                        if (String.IsNullOrEmpty(option))
                        {
                            option = trnVariant.ValueName;
                        }
                        else
                        {
                            option = option + ", " + trnVariant.ValueName;
                        }
                    }

                    populateSkuResponse.VariantOptions = option;

                    resp.Add(populateSkuResponse);
                }

                return resp;
            }
            else if (req.Options.Count == 3)
            {
                var trnProductVariantOptions = new List<TrnProductVariantOption>();
                var trnProductSku = new List<TrnProductSku>();
                var trnProductSkuValues = new List<TrnProductSkuValue>();

                foreach (var item in req.Options)
                {

                    var counter = 1;

                    foreach (var value in item.OptionValue)
                    {
                        trnProductVariantOptions.Add(new TrnProductVariantOption
                        {
                            ProductId = productId,
                            VariantId = item.VariantId,
                            ValueId = counter,
                            ValueName = value
                        });
                        counter += 1;
                    }
                };

                var skuCounter = 1;
                foreach (var item in req.Options[0].OptionValue)
                {
                    var trnProductVariantOption = trnProductVariantOptions.Find(x => x.ValueName == item);

                    foreach (var value in req.Options[1].OptionValue)
                    {
                        var trnProductVariantOptionTwo = trnProductVariantOptions.Find(x => x.ValueName == value);

                        foreach (var val in req.Options[2].OptionValue)
                        {
                            trnProductSkuValues.Add(new TrnProductSkuValue
                            {
                                ProductId = productId,
                                SkuId = skuCounter,
                                ValueId = trnProductVariantOption.ValueId,
                                VariantId = trnProductVariantOption.VariantId
                            });

                            trnProductSkuValues.Add(new TrnProductSkuValue
                            {
                                ProductId = productId,
                                SkuId = skuCounter,
                                ValueId = trnProductVariantOptionTwo.ValueId,
                                VariantId = trnProductVariantOptionTwo.VariantId
                            });

                            var trnProductVariantOptionThree = trnProductVariantOptions.Find(x => x.ValueName == val);

                            trnProductSkuValues.Add(new TrnProductSkuValue
                            {
                                ProductId = productId,
                                SkuId = skuCounter,
                                ValueId = trnProductVariantOptionThree.ValueId,
                                VariantId = trnProductVariantOptionThree.VariantId
                            });

                            skuCounter += 1;
                        }

                    }
                }

                trnProductSku =
                    (from row in trnProductSkuValues
                     group row by row.SkuId
                    into g
                     select new TrnProductSku()
                     {
                         ProductId = productId,
                         SkuId = (from res in g select res.SkuId).FirstOrDefault()
                     }).ToList();

                trnProductVariantOptions.ForEach(x => _context.TrnProductVariantOption.AddAsync(x));
                trnProductSku.ForEach(x => _context.TrnProductSku.AddAsync(x));
                trnProductSkuValues.ForEach(x => _context.TrnProductSkuValue.AddAsync(x));

                await _context.SaveChangesAsync();

                var totalSku = trnProductSku.Count;

                var resp = new List<PopulateSkuResponse>();
                for (int i = 1; i <= totalSku; i++)
                {
                    var populateSkuResponse = new PopulateSkuResponse()
                    {
                        ProductId = productId,
                        SkuId = i
                    };

                    var trnProductSkuValue = await _context
                        .TrnProductSkuValue.Where(x => x.ProductId == productId && x.SkuId == i).ToListAsync();

                    var option = "";
                    foreach (var item in trnProductSkuValue)
                    {
                        var trnVariant = await _context.TrnProductVariantOption
                            .Where(x => x.ProductId == productId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                            .FirstOrDefaultAsync();

                        if (String.IsNullOrEmpty(option))
                        {
                            option = trnVariant.ValueName;
                        }
                        else
                        {
                            option = option + ", " + trnVariant.ValueName;
                        }
                    }

                    populateSkuResponse.VariantOptions = option;

                    resp.Add(populateSkuResponse);
                }

                return resp;
            }
            else
            {
                return null;
            }
        }
        public async Task<ResponseStatus> CreateProduct(CreateProductRequest req, List<ImageUrlResponse> imageUrlList,int currentLoginID)
        {
            using(var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region Product

                    var trnProduct = await _context.TrnProduct
                                .Where(x => x.Id == req.ProductId)
                                .FirstOrDefaultAsync();

                    //just in case
                    if(req.BrandId==0)
                    {
                        req.BrandId=2;
                    }

                    var productToAdd = new Product()
                    {
                        Name = req.Name,
                        IsActive = true,
                        CreatedDate = DateTime.Now,
                        CreatedBy = currentLoginID,
                        ProductCategoryId = trnProduct.ProductCategoryId,
                        Description = req.Description,
                        BrandId = req.BrandId,
                    };
                    await _context.Product.AddAsync(productToAdd);
                    await _context.SaveChangesAsync();

                    #endregion

                    #region Image

                    if (imageUrlList.Count > 0)
                    {
                        bool photoIsMain = true;          
                        foreach (var img in imageUrlList)
                        {                   
                            var productImage = new ProductImage()
                            {
                                Url = img.ImgPath,
                                ThumbnailUrl = img.ThumbnailPath,
                                isMain = photoIsMain,
                                CreatedDate = DateTime.Now,
                                CreatedBy = currentLoginID,
                                ProductId = productToAdd.Id
                            };
                            await _context.ProductImage.AddAsync(productImage);
                            photoIsMain=false;                    
                        }
                    }

                    #endregion

                    #region Price

                    var productPrice = new ProductPrice()
                    {
                        Price = req.Price,
                        isActive = true,
                        StartDate = DateTime.Now,
                        ProductId = productToAdd.Id
                    };
                    await _context.ProductPrice.AddAsync(productPrice);

                    #endregion
                
                    #region Promotion

                    if(req.Promotion>0)
                    {
                    double discountPrice= double.Parse((((double)req.Promotion/(double)100)*(double)req.Price).ToString("0.00"));

                    var productPromote = new ProductPromotion()
                    {
                        Percent = req.Promotion,
                        FixedAmt = 0,
                        TotalAmt = req.Price-discountPrice,
                        ProductId = productToAdd.Id,
                        CreatedDate=DateTime.Now,
                        CreatedBy=currentLoginID
                    };
                    await _context.ProductPromotion.AddAsync(productPromote);

                    }
                    
                    #endregion

                    #region Tags

                    foreach(var tag in req.TagsList)
                    {
                        if(tag.Id==0)
                        {
                            Tag t=new Tag(){
                            };
                            if(!_context.Tag.Any(x=>x.Name==tag.Name))
                            {
                                t.Name=tag.Name;
                                _context.Tag.Add(t);
                                await _context.SaveChangesAsync();
                            }else{
                                t = await _context.Tag.Where(x=> x.Name == tag.Name).FirstOrDefaultAsync();
                            }
                            ProductTag pTag=new ProductTag(){
                            ProductId=productToAdd.Id,
                            TagId=t.Id
                            };
                            await _context.ProductTag.AddAsync(pTag);
                        }
                        else{
                            ProductTag pTag=new ProductTag(){
                            ProductId=productToAdd.Id,
                            TagId=tag.Id
                        };
                        await _context.ProductTag.AddAsync(pTag);
                        }
                        
                    }
                    await _context.SaveChangesAsync();
                    #endregion
                    
                    #region Clip

                    ProductClip clip=new ProductClip(){
                        ProductId=productToAdd.Id,
                        Name=req.ProductClip.Name,
                        ClipPath=req.ProductClip.ClipPath,
                        SeqNo=req.ProductClip.SeqNo
                    };
                    await _context.ProductClip.AddAsync(clip);

                    #endregion
                    
                    #region Sku

                    #region Update Sku qty

                    foreach (var item in req.Sku)
                    {
                        var sku = await _context
                                .TrnProductSku.Where(x => x.SkuId == item.SkuId 
                                && x.ProductId == req.ProductId)
                                .FirstOrDefaultAsync();
                            sku.Qty = item.Qty;

                        if(item.Price==0)
                        {
                            item.Price=req.Price;
                        }
                    }
                    await _context.SaveChangesAsync();

                    #endregion 
                    
                    #region Remove Sku where Qty = 0

                    var skuToRemove = await _context.TrnProductSku
                                    .Where(x => x.ProductId == req.ProductId 
                                    && x.Qty == 0)
                                    .ToArrayAsync();

                    var arrySku = skuToRemove.Select(x=>x.SkuId).ToList();           

                    var skuValueToRemove = await _context.TrnProductSkuValue
                                        .Where(x => x.ProductId == req.ProductId
                                        && arrySku.Contains(x.SkuId))
                                        .ToListAsync();

                    _context.TrnProductSku.RemoveRange(skuToRemove);
                    _context.TrnProductSkuValue.RemoveRange(skuValueToRemove);

                    #endregion
                    
                    #region Add product variant, Sku and Sku value

                    var productVariantOptionToAdd = await _context.TrnProductVariantOption
                                                    .Where(x => x.ProductId == req.ProductId)
                                                    .Select(z => new ProductVariantOption
                                                    {
                                                        ProductId = productToAdd.Id,
                                                        VariantId = z.VariantId,
                                                        ValueId = z.ValueId,
                                                        ValueName = z.ValueName
                                                    }).ToListAsync();

                    var productSkuToAdd = await _context.TrnProductSku
                                        .Where(x => x.ProductId == req.ProductId)
                                        .Select(z => new ProductSku
                                        {
                                            ProductId = productToAdd.Id,
                                            SkuId = z.SkuId,
                                            Qty = z.Qty,
                                            Price=0,//req.Sku.Where(sku=>sku.SkuId==z.SkuId).Select(sku=>sku.Price).FirstOrDefault(),
                                            CreatedDate = DateTime.Now,
                                            CreatedBy = currentLoginID
                                        }).ToListAsync();

                    foreach(var sku in productSkuToAdd)
                    {
                        sku.Price=req.Sku.Where(sku=>sku.SkuId==sku.SkuId).Select(sku=>sku.Price).FirstOrDefault();
                    }

                    var productSkuValueToAdd = await _context.TrnProductSkuValue
                                            .Where(x => x.ProductId == req.ProductId)
                                            .Select(z => new ProductSkuValue
                                            {
                                                ProductId = productToAdd.Id,
                                                SkuId = z.SkuId,
                                                VariantId = z.VariantId,
                                                ValueId = z.ValueId
                                            }).ToListAsync();

                    await _context.ProductVariantOption.AddRangeAsync(productVariantOptionToAdd);
                    await _context.ProductSku.AddRangeAsync(productSkuToAdd);
                    await _context.ProductSkuValue.AddRangeAsync(productSkuValueToAdd);

                    #endregion
            
                    #region Remove product variant, Sku and Sku value

                    var trnProductVariantOptionToRemove = await _context.TrnProductVariantOption
                                                        .Where(x => x.ProductId == req.ProductId)
                                                        .ToArrayAsync();

                    var trnProductSkuToRemove = await _context.TrnProductSku
                                                .Where(x => x.ProductId == req.ProductId)
                                                .ToArrayAsync();

                    var trnProductSkuValueToRemove = await _context.TrnProductSkuValue
                                                .Where(x => x.ProductId == req.ProductId)
                                                .ToListAsync();

                    _context.TrnProduct.Remove(trnProduct);
                    _context.TrnProductVariantOption.RemoveRange(trnProductVariantOptionToRemove);
                    _context.TrnProductSku.RemoveRange(trnProductSkuToRemove);
                    _context.TrnProductSkuValue.RemoveRange(trnProductSkuValueToRemove);

                    #endregion
                    
                    #endregion
                    
                    #region Save changes
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    #endregion
                    
                    return new ResponseStatus{
                        StatusCode=StatusCodes.Status200OK,
                        Message="အောင်မြင်သည်။"
                    };
                }
                catch (Exception e)
                {
                    log.Error(string.Format("error => {0}, inner exception => {1} ",e.Message,e.InnerException.Message));
                    transaction.Rollback();
                    return new ResponseStatus{
                        StatusCode=StatusCodes.Status500InternalServerError,
                        Message="မအောင်မြင်ပါ။"
                    };
                }
                
            }
        }
        public async Task<ResponseStatus> UpdateProduct(UpdateProductRequest req, List<ImageUrlResponse> imageUrlList,int currentLoginID)        
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    #region Product

                    var product= await _context.Product.Where(x=>x.Id==req.ProductId).SingleOrDefaultAsync();
                    product.Name=req.Name;
                    product.Description=req.Description;
                    product.UpdatedDate=DateTime.Now;
                    product.UpdatedBy=currentLoginID;
                    product.BrandId = req.BrandId;
                    
                    #endregion

                    #region Image

                    if (imageUrlList.Count > 0)
                    {     
                        foreach (var img in imageUrlList)
                        {                   
                            switch(img.Action)
                            {
                                case "New" : {
                                    var productImage = new ProductImage()
                                    {
                                        Url = img.ImgPath,
                                        ThumbnailUrl = img.ThumbnailPath,
                                        isMain = false,
                                        CreatedDate = DateTime.Now,
                                        CreatedBy = currentLoginID,
                                        ProductId = req.ProductId
                                    };
                                    await _context.ProductImage.AddAsync(productImage);
                                }; break;

                                case "Edit" : {
                                    var productImage= await _context.ProductImage.Where(x=>x.Id==img.ImageId && x.ProductId==req.ProductId).SingleOrDefaultAsync();
                                    productImage.Url = img.ImgPath;
                                    productImage.ThumbnailUrl = img.ThumbnailPath;
                                    productImage.UpdatedDate=DateTime.Now;
                                    productImage.UpdatedBy=currentLoginID;
                                }; break;

                                case "Delete" : {
                                    var productImage= await _context.ProductImage.Where(x=>x.Id==img.ImageId && x.ProductId==req.ProductId).SingleOrDefaultAsync();
                                    _context.ProductImage.Remove(productImage);
                                }; break;
                            }                                        
                        }
                    }

                     await _context.SaveChangesAsync();
                     
                    // set isMain if there's no main photo
                    if(!_context.ProductImage.Any(x=>x.ProductId==req.ProductId && x.isMain==true))
                    {
                        var isMainProduct=await _context.ProductImage.Where(x=>x.ProductId==req.ProductId).FirstOrDefaultAsync();
                        isMainProduct.isMain=true;
                        await _context.SaveChangesAsync();
                    }


                    #endregion

                    #region Price

                    var productPrice = await _context.ProductPrice.Where(x=>x.Id==req.PriceId && x.ProductId==req.ProductId).SingleOrDefaultAsync();
                    productPrice.Price=req.Price;

                    #endregion
                
                    #region Promotion

                    if(req.PromotionId>0)
                    {

                        var productPromote= await _context.ProductPromotion.Where(x=>x.Id==req.PromotionId && x.ProductId==req.ProductId).SingleOrDefaultAsync();
                        if (req.Promotion > 0)
                        {
                            double discountPrice= double.Parse((((double)req.Promotion/(double)100)*(double)req.Price).ToString("0.00"));
                            productPromote.Percent = req.Promotion;
                            productPromote.FixedAmt = discountPrice;
                            productPromote.TotalAmt = req.Price-discountPrice;
                            productPromote.UpdatedDate=DateTime.Now;
                            productPromote.UpdatedBy=currentLoginID;
                        }else{
                            _context.ProductPromotion.RemoveRange(productPromote);
                            await _context.SaveChangesAsync();
                        }
                    
                    }

                    else if(req.Promotion>0)
                    {
                        var promotion  = await _context.ProductPromotion.Where(x => x.ProductId == req.ProductId).SingleOrDefaultAsync();
                        if (promotion != null)
                        {
                            _context.ProductPromotion.RemoveRange(promotion);
                            await _context.SaveChangesAsync();
                        }
                        double discountPrice= double.Parse((((double)req.Promotion/(double)100)*(double)req.Price).ToString("0.00"));

                        var productPromote = new ProductPromotion()
                        {
                            Percent = req.Promotion,
                            FixedAmt = discountPrice,
                            TotalAmt = req.Price-discountPrice,
                            ProductId = req.ProductId,
                            CreatedDate=DateTime.Now,
                            CreatedBy=currentLoginID
                        };
                        await _context.ProductPromotion.AddAsync(productPromote);

                    }
                    #endregion

                    #region Tags

                    var tagToRemove= await _context.ProductTag.Where(x=>x.ProductId==req.ProductId).ToListAsync();
                    _context.ProductTag.RemoveRange(tagToRemove);

                    // foreach(var tag in req.TagsList)
                    // {
                    //     if(tag.Id==0)
                    //     {
                    //         Tag t=new Tag(){
                    //             Name=tag.Name,
                    //         };
                    //         _context.Tag.Add(t);
                    //         await _context.SaveChangesAsync();
                            
                    //         ProductTag pTag=new ProductTag(){
                    //         ProductId=product.Id,
                    //         TagId=t.Id
                    //         };
                    //         await _context.ProductTag.AddAsync(pTag);

                    //     }
                    //     else{
                    //         ProductTag pTag=new ProductTag(){
                    //         ProductId=product.Id,
                    //         TagId=tag.Id
                    //     };
                    //     await _context.ProductTag.AddAsync(pTag);
                    //     }
                    // }
                    foreach(var tag in req.TagsList)
                    {
                        if(tag.Id==0)
                        {
                            Tag t=new Tag(){
                            };
                            if(!_context.Tag.Any(x=>x.Name==tag.Name))
                            {
                                t.Name=tag.Name;
                                _context.Tag.Add(t);
                                await _context.SaveChangesAsync();
                            }else{
                                t = await _context.Tag.Where(x=> x.Name == tag.Name).FirstOrDefaultAsync();
                            }
                            ProductTag pTag=new ProductTag(){
                            ProductId=product.Id,
                            TagId=t.Id
                            };
                            await _context.ProductTag.AddAsync(pTag);
                        }
                        else{
                            // if (!_context.ProductTag.Any(x => x.ProductId == product.Id && x.TagId == tag.Id))
                            // {
                                
                                ProductTag pTag=new ProductTag(){
                                ProductId=product.Id,
                                TagId=tag.Id
                                };
                                
                                await _context.ProductTag.AddAsync(pTag);
                            // }
                        };
                    }
                        
                    
                    #endregion
                    
                    #region Clip

                    var clip= await _context.ProductClip.Where(x=>x.ProductId==req.ProductId).SingleOrDefaultAsync();
                    //clip.Name=req.ProductClip.Name;
                    clip.ClipPath=req.ProductClip.ClipPath;
                    clip.SeqNo=req.ProductClip.SeqNo;

                    #endregion
                    
                    #region Sku
                    foreach (var item in req.Sku)
                    {
                        if(item.Price==0)
                        {
                            item.Price=req.Price;
                        }

                        var sku = await _context.ProductSku.Where(x => x.SkuId == item.SkuId && x.ProductId == req.ProductId).FirstOrDefaultAsync();
                        sku.Qty = item.Qty;
                        sku.Price=item.Price;
                    }
                    #endregion

                    #region Save changes
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    #endregion
                    
                    return new ResponseStatus{
                        StatusCode=StatusCodes.Status200OK,
                        Message="အောင်မြင်သည်။"
                    };
                
                }
                catch (Exception e)
                {
                    log.Error(e.Message);
                    transaction.Rollback();
                    return new ResponseStatus{
                        StatusCode=StatusCodes.Status500InternalServerError,
                        Message="မအောင်မြင်ပါ။"
                    };
                }
            }
        }  
        public async Task<AddSkuForUpdateProductResponse> AddSkuForUpdateProduct(AddSkuForUpdateProductRequest request,int currentLoginID)
        {
            var trnProductVariantOptions = new List<ProductVariantOption>();
            var productSkuList = new List<ProductSku>();
            var productSkuValues = new List<ProductSkuValue>();
            AddSkuForUpdateProductResponse response = new AddSkuForUpdateProductResponse();
            if (request.VariantList.Count == 2)
            {
                var productVariantOptionRes1 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[0].VariantId && x.ValueName.Contains(request.VariantList[0].VariantName)).FirstOrDefaultAsync();
                if (productVariantOptionRes1 != null)
                {
                    var productVariantOptionRes2 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[1].VariantId && x.ValueName.Contains(request.VariantList[1].VariantName)).FirstOrDefaultAsync();
                    if (productVariantOptionRes2 != null)
                    {
                        var productSkuValueReq1 = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.ValueId == productVariantOptionRes1.ValueId && x.VariantId == productVariantOptionRes1.VariantId).Select(x => x.SkuId).ToListAsync();

                        var productSkuValueReq2 = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.ValueId == productVariantOptionRes2.ValueId && x.VariantId == productVariantOptionRes2.VariantId).Select(x => x.SkuId).ToListAsync();

                        List<int> list = new List<int>();
                        foreach (var item in productSkuValueReq1)
                        {
                            list.Add(item);
                        }
                        foreach (var item1 in productSkuValueReq2)
                        {
                            list.Add(item1);
                        }

                        
                        IEnumerable<int> duplicates = list.GroupBy(x => x)
                                    .Where(g => g.Count() > 1)
                                    .Select(x => x.Key);
                        if (duplicates?.Any() == true)
                        {
                            response.Message = "Same Sku";
                            response.StatusCode=StatusCodes.Status400BadRequest;
                            return response;
                        }
                        else
                        {
                            var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            ProductSkuValue producSkuValue2 = new ProductSkuValue();
                            producSkuValue2.ProductId = request.ProductId;
                            producSkuValue2.SkuId = productSkuValueId.SkuId + 1;
                            producSkuValue2.ValueId = productVariantOptionRes2.ValueId;
                            producSkuValue2.VariantId = request.VariantList[1].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue2);
                            productSkuValues.Add(producSkuValue2);

                            ProductSkuValue producSkuValue1 = new ProductSkuValue();
                            producSkuValue1.ProductId = request.ProductId;
                            producSkuValue1.SkuId = productSkuValueId.SkuId + 1;
                            producSkuValue1.ValueId = productVariantOptionRes1.ValueId;
                            producSkuValue1.VariantId = request.VariantList[0].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue1);
                            productSkuValues.Add(producSkuValue1);

                            await _context.SaveChangesAsync();


                            productSkuList =
                      (from row in productSkuValues
                       group row by row.SkuId
                      into g
                       select new ProductSku()
                       {
                           ProductId = request.ProductId,
                           SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                           CreatedDate = DateTime.Now,
                           CreatedBy = currentLoginID
                       }).ToList();

                            productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                            await _context.SaveChangesAsync();
                            var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            var updateSkuResponse = new AddSkuForUpdateProductResponse()
                            {
                                ProductId = request.ProductId,
                                SkuId = productId.SkuId
                            };

                            var trnProductSkuValue = await _context
                                .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                            var option = "";
                            foreach (var item in trnProductSkuValue)
                            {
                                var trnVariant = await _context.ProductVariantOption
                                    .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                    .FirstOrDefaultAsync();

                                if (String.IsNullOrEmpty(option))
                                {
                                    option = trnVariant.ValueName;
                                }
                                else
                                {
                                    option = option + ", " + trnVariant.ValueName;
                                }
                            }
                            updateSkuResponse.VariantOptions = option;
                            updateSkuResponse.Message = "Success";
                            updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                            return updateSkuResponse;
                        }
                    }
                    else
                    {
                        var productVariantOptionValueId = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[1].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                        ProductVariantOption productVariantOption = new ProductVariantOption();
                        productVariantOption.ProductId = request.ProductId;
                        productVariantOption.VariantId = request.VariantList[1].VariantId;
                        productVariantOption.ValueId = productVariantOptionValueId.ValueId + 1;
                        productVariantOption.ValueName = request.VariantList[1].VariantName;
                        await _context.ProductVariantOption.AddAsync(productVariantOption);

                        var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                        ProductSkuValue producSkuValue2 = new ProductSkuValue();
                        producSkuValue2.ProductId = request.ProductId;
                        producSkuValue2.SkuId = productSkuValueId.SkuId + 1;
                        producSkuValue2.ValueId = productVariantOption.ValueId;
                        producSkuValue2.VariantId = request.VariantList[1].VariantId;
                        await _context.ProductSkuValue.AddAsync(producSkuValue2);
                        productSkuValues.Add(producSkuValue2);


                        ProductSkuValue producSkuValue1 = new ProductSkuValue();
                        producSkuValue1.ProductId = request.ProductId;
                        producSkuValue1.SkuId = productSkuValueId.SkuId + 1;
                        producSkuValue1.ValueId = productVariantOptionRes1.ValueId;
                        producSkuValue1.VariantId = request.VariantList[0].VariantId;
                        await _context.ProductSkuValue.AddAsync(producSkuValue1);
                        productSkuValues.Add(producSkuValue1);

                        await _context.SaveChangesAsync();


                        productSkuList =
                  (from row in productSkuValues
                   group row by row.SkuId
                  into g
                   select new ProductSku()
                   {
                       ProductId = request.ProductId,
                       SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                       CreatedDate = DateTime.Now,
                       CreatedBy = currentLoginID
                   }).ToList();

                        productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                        await _context.SaveChangesAsync();

                        var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                        var updateSkuResponse = new AddSkuForUpdateProductResponse()
                        {
                            ProductId = request.ProductId,
                            SkuId = productId.SkuId
                        };

                        var trnProductSkuValue = await _context
                            .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                        var option = "";
                        foreach (var item in trnProductSkuValue)
                        {
                            var trnVariant = await _context.ProductVariantOption
                                .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                .FirstOrDefaultAsync();

                            if (String.IsNullOrEmpty(option))
                            {
                                option = trnVariant.ValueName;
                            }
                            else
                            {
                                option = option + ", " + trnVariant.ValueName;
                            }
                        }
                        updateSkuResponse.VariantOptions = option;
                        updateSkuResponse.Message = "Success";
                        updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                        return updateSkuResponse;
                    }

                }
                else
                {
                    var productVariantOptionValueId = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[0].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                    ProductVariantOption productVariantOption = new ProductVariantOption();
                    productVariantOption.ProductId = request.ProductId;
                    productVariantOption.VariantId = request.VariantList[0].VariantId;
                    productVariantOption.ValueId = productVariantOptionValueId.ValueId + 1;
                    productVariantOption.ValueName = request.VariantList[0].VariantName;
                    await _context.ProductVariantOption.AddAsync(productVariantOption);

                    var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                    ProductSkuValue producSkuValue1 = new ProductSkuValue();
                    producSkuValue1.ProductId = request.ProductId;
                    producSkuValue1.SkuId = productSkuValueId.SkuId + 1;
                    producSkuValue1.ValueId = productVariantOptionValueId.ValueId + 1;
                    producSkuValue1.VariantId = request.VariantList[0].VariantId;
                    await _context.ProductSkuValue.AddAsync(producSkuValue1);
                    productSkuValues.Add(producSkuValue1);

                    var productVariantOptionRes2 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[1].VariantId && x.ValueName.Contains(request.VariantList[1].VariantName)).FirstOrDefaultAsync();
                    if (productVariantOptionRes2 != null)
                    {
                        ProductSkuValue producSkuValue2 = new ProductSkuValue();
                        producSkuValue2.ProductId = request.ProductId;
                        producSkuValue2.SkuId = productSkuValueId.SkuId + 1;
                        producSkuValue2.ValueId = productVariantOptionRes2.ValueId;
                        producSkuValue2.VariantId = request.VariantList[1].VariantId;
                        await _context.ProductSkuValue.AddAsync(producSkuValue2);
                        productSkuValues.Add(producSkuValue2);

                        await _context.SaveChangesAsync();


                        productSkuList =
                  (from row in productSkuValues
                   group row by row.SkuId
                  into g
                   select new ProductSku()
                   {
                       ProductId = request.ProductId,
                       SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                       CreatedDate = DateTime.Now,
                       CreatedBy = currentLoginID
                   }).ToList();

                        productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                        await _context.SaveChangesAsync();
                        var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                        var updateSkuResponse = new AddSkuForUpdateProductResponse()
                        {
                            ProductId = request.ProductId,
                            SkuId = productId.SkuId
                        };

                        var trnProductSkuValue = await _context
                            .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                        var option = "";
                        foreach (var item in trnProductSkuValue)
                        {
                            var trnVariant = await _context.ProductVariantOption
                                .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                .FirstOrDefaultAsync();

                            if (String.IsNullOrEmpty(option))
                            {
                                option = trnVariant.ValueName;
                            }
                            else
                            {
                                option = option + ", " + trnVariant.ValueName;
                            }
                        }
                        updateSkuResponse.VariantOptions = option;
                        updateSkuResponse.Message = "Success";
                        updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                        return updateSkuResponse;
                    }
                    else
                    {
                        var productVariantOptionValueId2 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[1].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                        ProductVariantOption productVariantOption2 = new ProductVariantOption();
                        productVariantOption2.ProductId = request.ProductId;
                        productVariantOption2.VariantId = request.VariantList[1].VariantId;
                        productVariantOption2.ValueId = productVariantOptionValueId2.ValueId + 1;
                        productVariantOption2.ValueName = request.VariantList[1].VariantName;
                        await _context.ProductVariantOption.AddAsync(productVariantOption2);

                        ProductSkuValue producSkuValue2 = new ProductSkuValue();
                        producSkuValue2.ProductId = request.ProductId;
                        producSkuValue2.SkuId = productSkuValueId.SkuId + 1;
                        producSkuValue2.ValueId = productVariantOption2.ValueId;
                        producSkuValue2.VariantId = request.VariantList[1].VariantId;
                        await _context.ProductSkuValue.AddAsync(producSkuValue2);

                        productSkuValues.Add(producSkuValue2);

                        await _context.SaveChangesAsync();


                        productSkuList =
                  (from row in productSkuValues
                   group row by row.SkuId
                  into g
                   select new ProductSku()
                   {
                       ProductId = request.ProductId,
                       SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                       CreatedDate = DateTime.Now,
                       CreatedBy = currentLoginID
                   }).ToList();
                        productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                        await _context.SaveChangesAsync();
                        var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                        var updateSkuResponse = new AddSkuForUpdateProductResponse()
                        {
                            ProductId = request.ProductId,
                            SkuId = productId.SkuId
                        };

                        var trnProductSkuValue = await _context
                            .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                        var option = "";
                        foreach (var item in trnProductSkuValue)
                        {
                            var trnVariant = await _context.ProductVariantOption
                                .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                .FirstOrDefaultAsync();

                            if (String.IsNullOrEmpty(option))
                            {
                                option = trnVariant.ValueName;
                            }
                            else
                            {
                                option = option + ", " + trnVariant.ValueName;
                            }
                        }
                        updateSkuResponse.VariantOptions = option;
                        updateSkuResponse.Message = "Success";
                        updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                        return updateSkuResponse;

                    }
                }
            }
            else if (request.VariantList.Count == 3)
            {
                var productVariantOptionRes1 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[0].VariantId && x.ValueName.Contains(request.VariantList[0].VariantName)).FirstOrDefaultAsync();
                if (productVariantOptionRes1 != null)
                {
                    var productVariantOptionRes2 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[1].VariantId && x.ValueName.Contains(request.VariantList[1].VariantName)).FirstOrDefaultAsync();
                    if (productVariantOptionRes2 != null)
                    {
                        var productVariantOptionRes3 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[2].VariantId && x.ValueName.Contains(request.VariantList[2].VariantName)).FirstOrDefaultAsync();
                        if (productVariantOptionRes3 != null)
                        {
                            var productSkuValueReq1 = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.ValueId == productVariantOptionRes1.ValueId && x.VariantId == productVariantOptionRes1.VariantId).Select(x => x.SkuId).ToListAsync();

                            var productSkuValueReq2 = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.ValueId == productVariantOptionRes2.ValueId && x.VariantId == productVariantOptionRes2.VariantId).Select(x => x.SkuId).ToListAsync();

                            var productSkuValueReq3 = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.ValueId == productVariantOptionRes3.ValueId && x.VariantId == productVariantOptionRes3.VariantId).Select(x => x.SkuId).ToListAsync();

                            List<int> list = new List<int>();
                            foreach (var item in productSkuValueReq1)
                            {
                                list.Add(item);
                            }
                            foreach (var item1 in productSkuValueReq2)
                            {
                                list.Add(item1);
                            }
                           
                            IEnumerable<int> duplicates = list.GroupBy(x => x)
                                        .Where(g => g.Count() > 1)
                                        .Select(x => x.Key);
                            List<int> list1 = new List<int>();
                            list1.AddRange(duplicates);
                            if (duplicates?.Any() == true)
                            {
                                foreach (var item2 in productSkuValueReq3)
                                {
                                    list1.Add(item2);
                                }
                                IEnumerable<int> duplicates1 = list1.GroupBy(x => x)
                                        .Where(g => g.Count() > 1)
                                        .Select(x => x.Key);
                                if (duplicates1?.Any() == true)
                                {
                                    response.Message = "Same Sku";
                                    response.StatusCode=StatusCodes.Status400BadRequest;
                                    return response;
                                }
                                else
                                {
                                    var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                                    ProductSkuValue producSkuValue2 = new ProductSkuValue();
                                    producSkuValue2.ProductId = request.ProductId;
                                    producSkuValue2.SkuId = productSkuValueId.SkuId + 1;
                                    producSkuValue2.ValueId = productVariantOptionRes2.ValueId;
                                    producSkuValue2.VariantId = request.VariantList[1].VariantId;
                                    await _context.ProductSkuValue.AddAsync(producSkuValue2);
                                    productSkuValues.Add(producSkuValue2);

                                    ProductSkuValue producSkuValue1 = new ProductSkuValue();
                                    producSkuValue1.ProductId = request.ProductId;
                                    producSkuValue1.SkuId = productSkuValueId.SkuId + 1;
                                    producSkuValue1.ValueId = productVariantOptionRes1.ValueId;
                                    producSkuValue1.VariantId = request.VariantList[0].VariantId;
                                    await _context.ProductSkuValue.AddAsync(producSkuValue1);
                                    productSkuValues.Add(producSkuValue1);

                                    ProductSkuValue producSkuValue3 = new ProductSkuValue();
                                    producSkuValue3.ProductId = request.ProductId;
                                    producSkuValue3.SkuId = productSkuValueId.SkuId + 1;
                                    producSkuValue3.ValueId = productVariantOptionRes3.ValueId;
                                    producSkuValue3.VariantId = request.VariantList[2].VariantId;
                                    await _context.ProductSkuValue.AddAsync(producSkuValue3);
                                    productSkuValues.Add(producSkuValue3);

                                    await _context.SaveChangesAsync();


                                    productSkuList =
                                    (from row in productSkuValues
                                     group row by row.SkuId
                                    into g
                                     select new ProductSku()
                                     {
                                         ProductId = request.ProductId,
                                         SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                                         CreatedDate = DateTime.Now,
                                         CreatedBy = currentLoginID
                                     }).ToList();

                                    productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                                    await _context.SaveChangesAsync();
                                    var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                                    var updateSkuResponse = new AddSkuForUpdateProductResponse()
                                    {
                                        ProductId = request.ProductId,
                                        SkuId = productId.SkuId
                                    };

                                    var trnProductSkuValue = await _context
                                        .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                                    var option = "";
                                    foreach (var item in trnProductSkuValue)
                                    {
                                        var trnVariant = await _context.ProductVariantOption
                                            .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                            .FirstOrDefaultAsync();

                                        if (String.IsNullOrEmpty(option))
                                        {
                                            option = trnVariant.ValueName;
                                        }
                                        else
                                        {
                                            option = option + ", " + trnVariant.ValueName;
                                        }
                                    }
                                    updateSkuResponse.VariantOptions = option;
                                    updateSkuResponse.Message = "Success";
                                    updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                                    return updateSkuResponse;
                                }

                            }
                            else
                            {
                                var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                                ProductSkuValue producSkuValue2 = new ProductSkuValue();
                                producSkuValue2.ProductId = request.ProductId;
                                producSkuValue2.SkuId = productSkuValueId.SkuId + 1;
                                producSkuValue2.ValueId = productVariantOptionRes2.ValueId;
                                producSkuValue2.VariantId = request.VariantList[1].VariantId;
                                await _context.ProductSkuValue.AddAsync(producSkuValue2);
                                productSkuValues.Add(producSkuValue2);

                                ProductSkuValue producSkuValue1 = new ProductSkuValue();
                                producSkuValue1.ProductId = request.ProductId;
                                producSkuValue1.SkuId = productSkuValueId.SkuId + 1;
                                producSkuValue1.ValueId = productVariantOptionRes1.ValueId;
                                producSkuValue1.VariantId = request.VariantList[0].VariantId;
                                await _context.ProductSkuValue.AddAsync(producSkuValue1);
                                productSkuValues.Add(producSkuValue1);

                                ProductSkuValue producSkuValue3 = new ProductSkuValue();
                                producSkuValue3.ProductId = request.ProductId;
                                producSkuValue3.SkuId = productSkuValueId.SkuId + 1;
                                producSkuValue3.ValueId = productVariantOptionRes3.ValueId;
                                producSkuValue3.VariantId = request.VariantList[2].VariantId;
                                await _context.ProductSkuValue.AddAsync(producSkuValue3);
                                productSkuValues.Add(producSkuValue3);

                                await _context.SaveChangesAsync();


                                productSkuList =
                                (from row in productSkuValues
                                 group row by row.SkuId
                                into g
                                 select new ProductSku()
                                 {
                                     ProductId = request.ProductId,
                                     SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                                     CreatedDate = DateTime.Now,
                                     CreatedBy = currentLoginID
                                 }).ToList();

                                productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                                await _context.SaveChangesAsync();
                                var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                                var updateSkuResponse = new AddSkuForUpdateProductResponse()
                                {
                                    ProductId = request.ProductId,
                                    SkuId = productId.SkuId
                                };

                                var trnProductSkuValue = await _context
                                    .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                                var option = "";
                                foreach (var item in trnProductSkuValue)
                                {
                                    var trnVariant = await _context.ProductVariantOption
                                        .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                        .FirstOrDefaultAsync();

                                    if (String.IsNullOrEmpty(option))
                                    {
                                        option = trnVariant.ValueName;
                                    }
                                    else
                                    {
                                        option = option + ", " + trnVariant.ValueName;
                                    }
                                }
                                updateSkuResponse.VariantOptions = option;
                                updateSkuResponse.Message = "Success";
                                updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                                return updateSkuResponse;
                            }
                        }
                        else
                        {
                            var productVariantOptionValueId = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[2].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                            ProductVariantOption productVariantOption = new ProductVariantOption();
                            productVariantOption.ProductId = request.ProductId;
                            productVariantOption.VariantId = request.VariantList[2].VariantId;
                            productVariantOption.ValueId = productVariantOptionValueId.ValueId + 1;
                            productVariantOption.ValueName = request.VariantList[2].VariantName;
                            await _context.ProductVariantOption.AddAsync(productVariantOption);

                            var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            ProductSkuValue producSkuValue2 = new ProductSkuValue();
                            producSkuValue2.ProductId = request.ProductId;
                            producSkuValue2.SkuId = productSkuValueId.SkuId + 1;
                            producSkuValue2.ValueId = productVariantOptionRes2.ValueId;
                            producSkuValue2.VariantId = request.VariantList[1].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue2);
                            productSkuValues.Add(producSkuValue2);

                            ProductSkuValue producSkuValue1 = new ProductSkuValue();
                            producSkuValue1.ProductId = request.ProductId;
                            producSkuValue1.SkuId = productSkuValueId.SkuId + 1;
                            producSkuValue1.ValueId = productVariantOptionRes1.ValueId;
                            producSkuValue1.VariantId = request.VariantList[0].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue1);
                            productSkuValues.Add(producSkuValue1);

                            ProductSkuValue producSkuValue3 = new ProductSkuValue();
                            producSkuValue3.ProductId = request.ProductId;
                            producSkuValue3.SkuId = productSkuValueId.SkuId + 1;
                            producSkuValue3.ValueId = productVariantOption.ValueId;
                            producSkuValue3.VariantId = request.VariantList[2].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue3);
                            productSkuValues.Add(producSkuValue3);

                            await _context.SaveChangesAsync();


                            productSkuList =
                            (from row in productSkuValues
                             group row by row.SkuId
                            into g
                             select new ProductSku()
                             {
                                 ProductId = request.ProductId,
                                 SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                                 CreatedDate = DateTime.Now,
                                 CreatedBy = currentLoginID
                             }).ToList();

                            productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                            await _context.SaveChangesAsync();
                            var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            var updateSkuResponse = new AddSkuForUpdateProductResponse()
                            {
                                ProductId = request.ProductId,
                                SkuId = productId.SkuId
                            };

                            var trnProductSkuValue = await _context
                                .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                            var option = "";
                            foreach (var item in trnProductSkuValue)
                            {
                                var trnVariant = await _context.ProductVariantOption
                                    .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                    .FirstOrDefaultAsync();

                                if (String.IsNullOrEmpty(option))
                                {
                                    option = trnVariant.ValueName;
                                }
                                else
                                {
                                    option = option + ", " + trnVariant.ValueName;
                                }
                            }
                            updateSkuResponse.VariantOptions = option;
                            updateSkuResponse.Message = "Success";
                            updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                            return updateSkuResponse;

                        }
                    }
                    else
                    {
                        var productVariantOptionValueId = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[1].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                        ProductVariantOption productVariantOption = new ProductVariantOption();
                        productVariantOption.ProductId = request.ProductId;
                        productVariantOption.VariantId = request.VariantList[1].VariantId;
                        productVariantOption.ValueId = productVariantOptionValueId.ValueId + 1;
                        productVariantOption.ValueName = request.VariantList[1].VariantName;
                        await _context.ProductVariantOption.AddAsync(productVariantOption);

                        var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                        ProductSkuValue producSkuValue2 = new ProductSkuValue();
                        producSkuValue2.ProductId = request.ProductId;
                        producSkuValue2.SkuId = productSkuValueId.SkuId + 1;
                        producSkuValue2.ValueId = productVariantOption.ValueId;
                        producSkuValue2.VariantId = request.VariantList[1].VariantId;
                        await _context.ProductSkuValue.AddAsync(producSkuValue2);
                        productSkuValues.Add(producSkuValue2);


                        ProductSkuValue producSkuValue1 = new ProductSkuValue();
                        producSkuValue1.ProductId = request.ProductId;
                        producSkuValue1.SkuId = productSkuValueId.SkuId + 1;
                        producSkuValue1.ValueId = productVariantOptionRes1.ValueId;
                        producSkuValue1.VariantId = request.VariantList[0].VariantId;
                        await _context.ProductSkuValue.AddAsync(producSkuValue1);
                        productSkuValues.Add(producSkuValue1);

                        await _context.SaveChangesAsync();

                        var productVariantOptionRes3 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[2].VariantId && x.ValueName.Contains(request.VariantList[2].VariantName)).FirstOrDefaultAsync();
                        if (productVariantOptionRes3 != null)
                        {
                            ProductSkuValue producSkuValue3 = new ProductSkuValue();
                            producSkuValue3.ProductId = request.ProductId;
                            producSkuValue3.SkuId = productSkuValueId.SkuId + 1;
                            producSkuValue3.ValueId = productVariantOptionRes3.ValueId;
                            producSkuValue3.VariantId = request.VariantList[2].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue3);
                            productSkuValues.Add(producSkuValue3);

                            await _context.SaveChangesAsync();


                            productSkuList =
                      (from row in productSkuValues
                       group row by row.SkuId
                      into g
                       select new ProductSku()
                       {
                           ProductId = request.ProductId,
                           SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                           CreatedDate = DateTime.Now,
                           CreatedBy = currentLoginID
                       }).ToList();

                            productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                            await _context.SaveChangesAsync();
                            var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            var updateSkuResponse = new AddSkuForUpdateProductResponse()
                            {
                                ProductId = request.ProductId,
                                SkuId = productId.SkuId
                            };

                            var trnProductSkuValue = await _context
                                .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                            var option = "";
                            foreach (var item in trnProductSkuValue)
                            {
                                var trnVariant = await _context.ProductVariantOption
                                    .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                    .FirstOrDefaultAsync();

                                if (String.IsNullOrEmpty(option))
                                {
                                    option = trnVariant.ValueName;
                                }
                                else
                                {
                                    option = option + ", " + trnVariant.ValueName;
                                }
                            }
                            updateSkuResponse.VariantOptions = option;
                            updateSkuResponse.Message = "Success";
                            updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                            return updateSkuResponse;
                        }
                        else
                        {
                            var productVariantOptionValueId3 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[2].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                            ProductVariantOption productVariantOption3 = new ProductVariantOption();
                            productVariantOption3.ProductId = request.ProductId;
                            productVariantOption3.VariantId = request.VariantList[2].VariantId;
                            productVariantOption3.ValueId = productVariantOptionValueId3.ValueId + 1;
                            productVariantOption3.ValueName = request.VariantList[2].VariantName;
                            await _context.ProductVariantOption.AddAsync(productVariantOption3);

                            ProductSkuValue producSkuValue3 = new ProductSkuValue();
                            producSkuValue3.ProductId = request.ProductId;
                            producSkuValue3.SkuId = productSkuValueId.SkuId + 1;
                            producSkuValue3.ValueId = productVariantOption3.ValueId;
                            producSkuValue3.VariantId = request.VariantList[1].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue3);

                            productSkuValues.Add(producSkuValue3);

                            await _context.SaveChangesAsync();


                            productSkuList =
                      (from row in productSkuValues
                       group row by row.SkuId
                      into g
                       select new ProductSku()
                       {
                           ProductId = request.ProductId,
                           SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                           CreatedDate = DateTime.Now,
                           CreatedBy = currentLoginID
                       }).ToList();
                            productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                            await _context.SaveChangesAsync();
                            var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            var updateSkuResponse = new AddSkuForUpdateProductResponse()
                            {
                                ProductId = request.ProductId,
                                SkuId = productId.SkuId
                            };

                            var trnProductSkuValue = await _context
                                .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                            var option = "";
                            foreach (var item in trnProductSkuValue)
                            {
                                var trnVariant = await _context.ProductVariantOption
                                    .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                    .FirstOrDefaultAsync();

                                if (String.IsNullOrEmpty(option))
                                {
                                    option = trnVariant.ValueName;
                                }
                                else
                                {
                                    option = option + ", " + trnVariant.ValueName;
                                }
                            }
                            updateSkuResponse.VariantOptions = option;
                            updateSkuResponse.Message = "Success";
                            updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                            return updateSkuResponse;

                        }
                    }

                }
                else
                {
                    var productVariantOptionValueId1 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[0].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                    ProductVariantOption productVariantOption1 = new ProductVariantOption();
                    productVariantOption1.ProductId = request.ProductId;
                    productVariantOption1.VariantId = request.VariantList[0].VariantId;
                    productVariantOption1.ValueId = productVariantOptionValueId1.ValueId + 1;
                    productVariantOption1.ValueName = request.VariantList[0].VariantName;
                    await _context.ProductVariantOption.AddAsync(productVariantOption1);

                    var productSkuValueId1 = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                    ProductSkuValue producSkuValue11 = new ProductSkuValue();
                    producSkuValue11.ProductId = request.ProductId;
                    producSkuValue11.SkuId = productSkuValueId1.SkuId + 1;
                    producSkuValue11.ValueId = productVariantOptionValueId1.ValueId + 1;
                    producSkuValue11.VariantId = request.VariantList[0].VariantId;
                    await _context.ProductSkuValue.AddAsync(producSkuValue11);
                    productSkuValues.Add(producSkuValue11);

                    await _context.SaveChangesAsync();

                    // var productVariantOptionRes11 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[0].VariantId && x.ValueName.Contains(request.VariantList[0].VariantName)).FirstOrDefaultAsync();
                    // if (productVariantOptionRes11 != null)
                    // {
                    var productVariantOptionRes2 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[1].VariantId && x.ValueName.Contains(request.VariantList[1].VariantName)).FirstOrDefaultAsync();
                    if (productVariantOptionRes2 != null)
                    {
                        //var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                        ProductSkuValue producSkuValue2 = new ProductSkuValue();
                        producSkuValue2.ProductId = request.ProductId;
                        producSkuValue2.SkuId = productSkuValueId1.SkuId + 1;
                        producSkuValue2.ValueId = productVariantOptionRes2.ValueId;
                        producSkuValue2.VariantId = request.VariantList[1].VariantId;
                        await _context.ProductSkuValue.AddAsync(producSkuValue2);
                        productSkuValues.Add(producSkuValue2);
                        await _context.SaveChangesAsync();

                        var productVariantOptionRes3 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[2].VariantId && x.ValueName.Contains(request.VariantList[2].VariantName)).FirstOrDefaultAsync();
                        if (productVariantOptionRes3 != null)
                        {
                            ProductSkuValue producSkuValue3 = new ProductSkuValue();
                            producSkuValue3.ProductId = request.ProductId;
                            producSkuValue3.SkuId = productSkuValueId1.SkuId + 1;
                            producSkuValue3.ValueId = productVariantOptionRes3.ValueId;
                            producSkuValue3.VariantId = request.VariantList[2].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue3);
                            productSkuValues.Add(producSkuValue3);

                            await _context.SaveChangesAsync();


                            productSkuList =
                            (from row in productSkuValues
                             group row by row.SkuId
                            into g
                             select new ProductSku()
                             {
                                 ProductId = request.ProductId,
                                 SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                                 CreatedDate = DateTime.Now,
                                 CreatedBy = currentLoginID
                             }).ToList();

                            productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                            await _context.SaveChangesAsync();
                            var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            var updateSkuResponse = new AddSkuForUpdateProductResponse()
                            {
                                ProductId = request.ProductId,
                                SkuId = productId.SkuId
                            };

                            var trnProductSkuValue = await _context
                                .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                            var option = "";
                            foreach (var item in trnProductSkuValue)
                            {
                                var trnVariant = await _context.ProductVariantOption
                                    .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                    .FirstOrDefaultAsync();

                                if (String.IsNullOrEmpty(option))
                                {
                                    option = trnVariant.ValueName;
                                }
                                else
                                {
                                    option = option + ", " + trnVariant.ValueName;
                                }
                            }
                            updateSkuResponse.VariantOptions = option;
                            updateSkuResponse.Message = "Success";
                            updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                            return updateSkuResponse;

                        }
                        else
                        {
                            var productVariantOptionValueId = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[2].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                            ProductVariantOption productVariantOption = new ProductVariantOption();
                            productVariantOption.ProductId = request.ProductId;
                            productVariantOption.VariantId = request.VariantList[2].VariantId;
                            productVariantOption.ValueId = productVariantOptionValueId.ValueId + 1;
                            productVariantOption.ValueName = request.VariantList[2].VariantName;
                            await _context.ProductVariantOption.AddAsync(productVariantOption);

                            ProductSkuValue producSkuValue3 = new ProductSkuValue();
                            producSkuValue3.ProductId = request.ProductId;
                            producSkuValue3.SkuId = productSkuValueId1.SkuId + 1;
                            producSkuValue3.ValueId = productVariantOption.ValueId;
                            producSkuValue3.VariantId = request.VariantList[2].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue3);
                            productSkuValues.Add(producSkuValue3);

                            await _context.SaveChangesAsync();


                            productSkuList =
                            (from row in productSkuValues
                             group row by row.SkuId
                            into g
                             select new ProductSku()
                             {
                                 ProductId = request.ProductId,
                                 SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                                 CreatedDate = DateTime.Now,
                                 CreatedBy = currentLoginID
                             }).ToList();

                            productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                            await _context.SaveChangesAsync();
                            var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            var updateSkuResponse = new AddSkuForUpdateProductResponse()
                            {
                                ProductId = request.ProductId,
                                SkuId = productId.SkuId
                            };

                            var trnProductSkuValue = await _context
                                .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                            var option = "";
                            foreach (var item in trnProductSkuValue)
                            {
                                var trnVariant = await _context.ProductVariantOption
                                    .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                    .FirstOrDefaultAsync();

                                if (String.IsNullOrEmpty(option))
                                {
                                    option = trnVariant.ValueName;
                                }
                                else
                                {
                                    option = option + ", " + trnVariant.ValueName;
                                }
                            }
                            updateSkuResponse.VariantOptions = option;
                            updateSkuResponse.Message = "Success";
                            updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                            return updateSkuResponse;

                        }
                    }
                    else
                    {
                        var productVariantOptionValueId = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[1].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                        ProductVariantOption productVariantOption = new ProductVariantOption();
                        productVariantOption.ProductId = request.ProductId;
                        productVariantOption.VariantId = request.VariantList[1].VariantId;
                        productVariantOption.ValueId = productVariantOptionValueId.ValueId + 1;
                        productVariantOption.ValueName = request.VariantList[1].VariantName;
                        await _context.ProductVariantOption.AddAsync(productVariantOption);

                        //var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                        ProductSkuValue producSkuValue2 = new ProductSkuValue();
                        producSkuValue2.ProductId = request.ProductId;
                        producSkuValue2.SkuId = productSkuValueId1.SkuId + 1;
                        producSkuValue2.ValueId = productVariantOption.ValueId;
                        producSkuValue2.VariantId = request.VariantList[1].VariantId;
                        await _context.ProductSkuValue.AddAsync(producSkuValue2);
                        productSkuValues.Add(producSkuValue2);


                        await _context.SaveChangesAsync();

                        var productVariantOptionRes3 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[2].VariantId && x.ValueName.Contains(request.VariantList[2].VariantName)).FirstOrDefaultAsync();
                        if (productVariantOptionRes3 != null)
                        {
                            ProductSkuValue producSkuValue3 = new ProductSkuValue();
                            producSkuValue3.ProductId = request.ProductId;
                            producSkuValue3.SkuId = productSkuValueId1.SkuId + 1;
                            producSkuValue3.ValueId = productVariantOptionRes3.ValueId;
                            producSkuValue3.VariantId = request.VariantList[2].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue3);
                            productSkuValues.Add(producSkuValue3);

                            await _context.SaveChangesAsync();


                            productSkuList =
                      (from row in productSkuValues
                       group row by row.SkuId
                      into g
                       select new ProductSku()
                       {
                           ProductId = request.ProductId,
                           SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                           CreatedDate = DateTime.Now,
                           CreatedBy = currentLoginID
                       }).ToList();

                            productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                            await _context.SaveChangesAsync();
                            var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            var updateSkuResponse = new AddSkuForUpdateProductResponse()
                            {
                                ProductId = request.ProductId,
                                SkuId = productId.SkuId
                            };

                            var trnProductSkuValue = await _context
                                .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                            var option = "";
                            foreach (var item in trnProductSkuValue)
                            {
                                var trnVariant = await _context.ProductVariantOption
                                    .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                    .FirstOrDefaultAsync();

                                if (String.IsNullOrEmpty(option))
                                {
                                    option = trnVariant.ValueName;
                                }
                                else
                                {
                                    option = option + ", " + trnVariant.ValueName;
                                }
                            }
                            updateSkuResponse.VariantOptions = option;
                            updateSkuResponse.Message = "Success";
                            updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                            return updateSkuResponse;
                        }
                        else
                        {
                            var productVariantOptionValueId3 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[2].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                            ProductVariantOption productVariantOption3 = new ProductVariantOption();
                            productVariantOption3.ProductId = request.ProductId;
                            productVariantOption3.VariantId = request.VariantList[2].VariantId;
                            productVariantOption3.ValueId = productVariantOptionValueId3.ValueId + 1;
                            productVariantOption3.ValueName = request.VariantList[2].VariantName;
                            await _context.ProductVariantOption.AddAsync(productVariantOption3);

                            ProductSkuValue producSkuValue3 = new ProductSkuValue();
                            producSkuValue3.ProductId = request.ProductId;
                            producSkuValue3.SkuId = productSkuValueId1.SkuId + 1;
                            producSkuValue3.ValueId = productVariantOption3.ValueId;
                            producSkuValue3.VariantId = request.VariantList[1].VariantId;
                            await _context.ProductSkuValue.AddAsync(producSkuValue3);

                            productSkuValues.Add(producSkuValue3);

                            await _context.SaveChangesAsync();


                            productSkuList =
                      (from row in productSkuValues
                       group row by row.SkuId
                      into g
                       select new ProductSku()
                       {
                           ProductId = request.ProductId,
                           SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                           CreatedDate = DateTime.Now,
                           CreatedBy = currentLoginID
                       }).ToList();
                            productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                            await _context.SaveChangesAsync();
                            var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                            var updateSkuResponse = new AddSkuForUpdateProductResponse()
                            {
                                ProductId = request.ProductId,
                                SkuId = productId.SkuId
                            };

                            var trnProductSkuValue = await _context
                                .ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                            var option = "";
                            foreach (var item in trnProductSkuValue)
                            {
                                var trnVariant = await _context.ProductVariantOption
                                    .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                                    .FirstOrDefaultAsync();

                                if (String.IsNullOrEmpty(option))
                                {
                                    option = trnVariant.ValueName;
                                }
                                else
                                {
                                    option = option + ", " + trnVariant.ValueName;
                                }
                            }
                            updateSkuResponse.VariantOptions = option;
                            updateSkuResponse.Message = "Success";
                            updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                            return updateSkuResponse;

                        }
                    }

                    //}
                }
            }
            else if (request.VariantList.Count == 1)
            {
                var productVariantOptionRes1 = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[0].VariantId && x.ValueName.Contains(request.VariantList[0].VariantName)).FirstOrDefaultAsync();
                if (productVariantOptionRes1 != null)
                {                    
                    response.StatusCode=StatusCodes.Status400BadRequest;
                    response.Message = "Same Sku";
                    return response;
                }
                else
                {
                    #region Add new Product Variant Option with increase ID

                    var productVariantOptionValueId = await _context.ProductVariantOption.Where(x => x.ProductId == request.ProductId && x.VariantId == request.VariantList[0].VariantId).OrderByDescending(x => x.ValueId).FirstOrDefaultAsync();
                    ProductVariantOption productVariantOption = new ProductVariantOption();
                    productVariantOption.ProductId = request.ProductId;
                    productVariantOption.VariantId = request.VariantList[0].VariantId;
                    productVariantOption.ValueId = productVariantOptionValueId.ValueId + 1;
                    productVariantOption.ValueName = request.VariantList[0].VariantName;
                    await _context.ProductVariantOption.AddAsync(productVariantOption);

                    #endregion

                    #region Add new Product Sku Value with increase ID

                     var productSkuValueId = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                    ProductSkuValue producSkuValue1 = new ProductSkuValue();
                    producSkuValue1.ProductId = request.ProductId;
                    producSkuValue1.SkuId = productSkuValueId.SkuId + 1;
                    producSkuValue1.ValueId = productVariantOptionValueId.ValueId + 1;
                    producSkuValue1.VariantId = request.VariantList[0].VariantId;
                    await _context.ProductSkuValue.AddAsync(producSkuValue1);
                    productSkuValues.Add(producSkuValue1);
                    await _context.SaveChangesAsync();

                    #endregion
                   
                    #region Add Product Sku

                     productSkuList =(from row in productSkuValues
                                    group row by row.SkuId
                                    into g
                                    select new ProductSku()
                                    {
                                        ProductId = request.ProductId,
                                        SkuId = (from res in g select res.SkuId).FirstOrDefault(),
                                        CreatedDate = DateTime.Now,
                                        CreatedBy = currentLoginID
                                    }).ToList();

                    productSkuList.ForEach(x => _context.ProductSku.AddAsync(x));
                    await _context.SaveChangesAsync();

                    #endregion

                   #region  select response
                    var productId = await _context.ProductSku.Where(x => x.ProductId == request.ProductId).OrderByDescending(x => x.SkuId).FirstOrDefaultAsync();
                    var updateSkuResponse = new AddSkuForUpdateProductResponse()
                    {
                        ProductId = request.ProductId,
                        SkuId = productId.SkuId
                    };

                    var trnProductSkuValue = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == productId.SkuId).ToListAsync();

                    var option = "";
                    foreach (var item in trnProductSkuValue)
                    {
                        var trnVariant = await _context.ProductVariantOption
                            .Where(x => x.ProductId == request.ProductId && x.VariantId == item.VariantId && x.ValueId == item.ValueId)
                            .FirstOrDefaultAsync();

                        if (String.IsNullOrEmpty(option))
                        {
                            option = trnVariant.ValueName;
                        }
                        else
                        {
                            option = option + ", " + trnVariant.ValueName;
                        }
                    }
                    updateSkuResponse.VariantOptions = option;
                    updateSkuResponse.Message = "Success";
                    updateSkuResponse.StatusCode=StatusCodes.Status200OK;
                    return updateSkuResponse;
                   #endregion
                   
                }
            }
            return response;
        }
        public async Task<ResponseStatus> DeleteSku(DeleteSkuRequest request)
        {
            ResponseStatus response = new ResponseStatus();
           
            var orderDetail = await _context.OrderDetail.Where(x => x.ProductId == request.ProductId && x.SkuId == request.SkuId).FirstOrDefaultAsync();
            if (orderDetail != null)
            {
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = "မအောင်မြင်ပါ။";                
            }
            else
            {
                var productSku = await _context.ProductSku.Where(x => x.ProductId == request.ProductId && x.SkuId == request.SkuId).FirstOrDefaultAsync();
                var skuValueToRemove = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId && x.SkuId == request.SkuId).ToListAsync();

                _context.ProductSku.RemoveRange(productSku);
                _context.ProductSkuValue.RemoveRange(skuValueToRemove);
                await _context.SaveChangesAsync();

                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "အောင်မြင်စွာ ပယ်ဖျက်ပြီးပါပြီ။";                
            }
            return response;
        }
        public async Task<ProductImage> GetProductImageById(int id)
        {
            return await _context.ProductImage.Where(x=>x.Id==id).SingleOrDefaultAsync();
        }
        public async Task<GetProductDetailResponse> GetProductDetail(GetProductDetailRequest request,int userId,string token)
        {
            var product= await _context.Product
                        .Include(x=>x.ProductPrice)                        
                        .Include(x=>x.ProductCategory)
                        .Where(x=>x.Id==request.ProductId)
                        .Select(x=> new GetProductDetailResponse{
                            Id=x.Id,
                            Name=x.Name,
                            Description=x.Description,
                            Price=x.ProductPrice.FirstOrDefault(p=>p.ProductId==request.ProductId).Price,
                            PriceId=x.ProductPrice.FirstOrDefault(p=>p.ProductId==request.ProductId).Id,                           
                            ProductCategoryId=x.ProductCategory.Id
                        }).SingleOrDefaultAsync();
            
            var category= await _context.ProductCategory.Where(x=>x.Id==product.ProductCategoryId).SingleOrDefaultAsync();
                                   
            List<GetPrdouctDetailCategoryResponse> proCat=new List<GetPrdouctDetailCategoryResponse>();
            GetPrdouctDetailCategoryResponse subCat= await _context.ProductCategory
                                                    .Where(x=>x.Id==category.Id)
                                                    .Select(x=>new GetPrdouctDetailCategoryResponse{
                                                    ProductCategoryId=x.Id,
                                                    ProductCategoryName=x.Name,
                                                    Url=x.Url,
                                                    IsMainCategory=false
                                                    }).SingleOrDefaultAsync();
            GetPrdouctDetailCategoryResponse mainCat= await _context.ProductCategory
                                                    .Where(x=>x.Id==category.SubCategoryId)
                                                    .Select(x=>new GetPrdouctDetailCategoryResponse{
                                                    ProductCategoryId=x.Id,
                                                    ProductCategoryName=x.Name,
                                                    Url=x.Url,
                                                    IsMainCategory=true
                                                    }).SingleOrDefaultAsync();
            proCat.Add(mainCat);
            proCat.Add(subCat);
            product.ProductCategory= proCat;

            var productPromote = await _context.ProductPromotion
                                                .Where(x => x.ProductId == product.Id)
                                                .FirstOrDefaultAsync();

                    if (productPromote != null)
                    {
                    product.PromotePrice = productPromote.TotalAmt;
                    product.PromotePercent = productPromote.Percent;
                    product.PromoteId=productPromote.Id;
                    }
                    else
                    {
                    product.PromotePrice = 0;
                    product.PromotePercent = 0;
                    product.PromoteId=0;
                    }


                   product.TagsList=await _context.ProductTag.Include(x=>x.Tag).Where(x=>x.ProductId==request.ProductId)
                                    .Select(x=>new Tag{
                                        Id=x.TagId,
                                        Name=x.Tag.Name
                                    }).ToListAsync();
                   product.ProductImage=await _context.ProductImage.Where(x=>x.ProductId==request.ProductId).ToListAsync();
                   product.ProductClip=await _context.ProductClip.Where(x=>x.ProductId==request.ProductId).SingleOrDefaultAsync();
                   product.ProductPromotion=await _context.ProductPromotion.Where(x=>x.ProductId==request.ProductId).SingleOrDefaultAsync();

                    var variantIds = await _context.ProductSkuValue.Where(x => x.ProductId == request.ProductId).Select(s => s.VariantId).Distinct().ToListAsync();

                    product.Variant = await _context.Variant.Where(x => x.ProductCategoryId == product.ProductCategoryId  && (variantIds.Count()==0 || variantIds.Contains(x.Id)))                   
                                    .Select(s => new GetProductDetailVariant
                                    {
                                        VariantId = s.Id,
                                        Name = s.Name
                                    }).ToListAsync();

                     var productSkuList = await _context.ProductSku.Where(x => x.ProductId == product.Id).ToListAsync();
                    if (productSkuList.Count() > 0)
                    {
                        product.SkuValue = new List<GetProductDetailSkuValue>();
                        foreach (var item in productSkuList)
                        {   
                            //check for duplicate multiple add tocart    
                            int cQty=0; 
                            if(userId!=0)
                            {
                            var userInfo = await _userServices.GetUserInfo(userId, token);                        
                            TrnCart cart=await _context.TrnCart.Where(a=>a.UserId==userId && a.SkuId==item.SkuId && a.ProductId==item.ProductId).FirstOrDefaultAsync();
                            
                            if(cart!=null && userInfo!=null && userInfo.UserTypeId==MayMayShopConst.USER_TYPE_BUYER){
                                cQty=cart.Qty;
                            }
                            }  
                            
                            product.Qty += item.Qty;
                            var skuKeys = await _context.ProductSkuValue
                                .Where(x => x.ProductId == item.ProductId && x.SkuId == item.SkuId)
                                .ToListAsync();
                            if (skuKeys.Count > 0)
                            {
                                #region GetPromotion
                               
                                int promotePercent=0;
                                double promotePrice=0;
                                if(productPromote!=null && productPromote.Percent>0)
                                {
                                    promotePercent=productPromote.Percent;
                                    double discountPrice= double.Parse((((double)productPromote.Percent/(double)100)*(double)item.Price).ToString("0.00"));
                                    promotePrice=item.Price-discountPrice;
                                }
                                
                                #endregion

                                // for (int i = 0; i < skuKeys.Count; i++)
                                // {
                                    var skuValue = await (from psku in _context.ProductSkuValue
                                                        from pvopt in _context.ProductVariantOption
                                                        where psku.ProductId == item.ProductId
                                                        && psku.SkuId == item.SkuId
                                                        && psku.ProductId == pvopt.ProductId
                                                        && psku.VariantId == pvopt.VariantId
                                                        && psku.ValueId == pvopt.ValueId
                                                        select pvopt.ValueName).ToListAsync();

                                    var skuValeForResp = new GetProductDetailSkuValue
                                    {
                                        SkuId = item.SkuId,
                                        Value = string.Join(",", skuValue),
                                        Qty = item.Qty-cQty,
                                        OriginalPrice=item.Price,
                                        PromotePercent=promotePercent,
                                        PromotePrice=promotePrice
                                    };

                                    product.SkuValue.Add(skuValeForResp);
                                // }
                            }
                        }
                    }
                    else
                    {
                        product.Qty = 0;
                        product.SkuValue = null;
                    }

            var req = new GetVariantValueRequest
            {
                ProductId = request.ProductId,
                CurrentVariantId = product.Variant[0].VariantId
            };

            product.VariantValues = await this.GetVariantValue(req);

            return product;
        }
         public async Task<List<GetVariantValueResponse>> GetVariantValue(GetVariantValueRequest request)
        {
            if (request.SelectedVariantId != 0)
            {
                var orderList = await (from proVarOpt in _context.ProductVariantOption
                                       where proVarOpt.ProductId == request.ProductId

                                       group new { proVarOpt } by new { proVarOpt.VariantId }
                                         into grp
                                       select grp.Key.VariantId).ToListAsync();

                var variantValues = await _context.ProductVariantOption
                    .Where(x => x.ProductId == request.ProductId &&
                    x.VariantId == request.CurrentVariantId).ToListAsync();

                var selectedVariantValue = await (
                    from pskuval in _context.ProductSkuValue
                    where pskuval.ProductId == request.ProductId
                    && pskuval.VariantId == request.SelectedVariantId
                    && pskuval.ValueId == request.SelectedValueId
                    select pskuval.SkuId).Distinct().ToListAsync();

                var availableValues = await (
                    from psku in _context.ProductSku
                    from pskuval in _context.ProductSkuValue
                    where psku.ProductId == request.ProductId
                    && pskuval.VariantId == request.CurrentVariantId
                    && selectedVariantValue.Contains(psku.SkuId)
                    && psku.Qty > 0
                    && psku.ProductId == pskuval.ProductId
                    && psku.SkuId == pskuval.SkuId
                    select pskuval.ValueId).Distinct().ToListAsync();


                var resp = variantValues.Where(x => availableValues.Contains(x.ValueId))
                    .Select(s => new GetVariantValueResponse
                    {
                        ValueId = s.ValueId,
                        ValueName = s.ValueName
                    }).ToList();

                if (orderList[orderList.Count - 1] == request.CurrentVariantId)
                {
                    var respWithQty = variantValues.Where(x => availableValues.Contains(x.ValueId))
                        .Select(s => new GetVariantValueResponse
                        {
                            ValueId = s.ValueId,
                            ValueName = s.ValueName
                        }).ToList();

                    return respWithQty;
                }

                return resp;
            }
            else
            {

                var orderList = await (from proVarOpt in _context.ProductVariantOption
                                       where proVarOpt.ProductId == request.ProductId

                                       group new { proVarOpt } by new { proVarOpt.VariantId }
                                         into grp
                                       select grp.Key.VariantId).ToListAsync();

                var variantValues = await _context.ProductVariantOption
                    .Where(x => x.ProductId == request.ProductId &&
                    x.VariantId == request.CurrentVariantId).ToListAsync();

                var productSkuHold = (
                    from prodVarOpt in variantValues
                    from prodSkuVal in _context.ProductSkuValue
                    where prodVarOpt.ProductId == prodSkuVal.ProductId
                    && prodVarOpt.VariantId == prodSkuVal.VariantId
                    && prodVarOpt.ValueId == prodSkuVal.ValueId
                    select prodSkuVal.SkuId
                ).Distinct().ToList();

                var availableValues = await (
                    from psku in _context.ProductSku.Where(x => productSkuHold.Contains(x.SkuId))
                    from pskuval in _context.ProductSkuValue
                    where psku.ProductId == request.ProductId
                    && pskuval.VariantId == request.CurrentVariantId
                    && psku.Qty > 0
                    && psku.ProductId == pskuval.ProductId
                    && psku.SkuId == pskuval.SkuId
                    select pskuval.ValueId).Distinct().ToListAsync();


                var resp = variantValues.Where(x => availableValues.Contains(x.ValueId))
                    .Select(s => new GetVariantValueResponse
                    {
                        ValueId = s.ValueId,
                        ValueName = s.ValueName
                    }).ToList();

                if (orderList[orderList.Count - 1] == request.CurrentVariantId)
                {
                    var respWithQty = variantValues.Where(x => availableValues.Contains(x.ValueId))
                        .Select(s => new GetVariantValueResponse
                        {
                            ValueId = s.ValueId,
                            ValueName = s.ValueName
                        }).ToList();

                    return respWithQty;
                }

                return resp;
            }

        }
        public async Task<List<GetLandingProductPromotionResponse>> GetLandingProductPromotion(GetLandingProductPromotionRequest request)
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

            var prodcutIDs=await _context.ProductPromotion
                            .Where(x=>!productSkuIDs.Contains(x.ProductId))                            
                            .Select(x=>x.ProductId)                            
                            .ToListAsync();
            return await _context.Product
                    .Include(x=>x.ProductPrice)
                    .Include(x=>x.ProductImage)
                    .Where(x=>prodcutIDs.Contains(x.Id)
                    && !productSkuIDs.Contains(x.Id)
                    && x.IsActive==true)
                    .OrderBy(x=>x.ProductPromotion.TotalAmt)
                    .Select(x=>new GetLandingProductPromotionResponse{
                        ProductId=x.Id,
                        Name=x.Name,
                        Url=x.ProductImage.Where(i=>i.ProductId==x.Id).Select(i=>i.Url).FirstOrDefault(),
                        OriginalPrice=x.ProductPrice.Where(i=>i.ProductId==x.Id).Select(i=>i.Price).FirstOrDefault(),
                        PromotePrice=x.ProductPromotion.TotalAmt,
                        PromotePercent=x.ProductPromotion.Percent
                    })
                    .Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize)
                    .ToListAsync();
        }
        public async Task<List<GetLandingProductLatestResponse>> GetLandingProductLatest(GetLandingProductLatestRequest request)
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

            List<GetLandingProductLatestResponse> response=new List<GetLandingProductLatestResponse>();

            var products= await _context.Product
                        .Include(x=>x.ProductImage)
                        .Include(x=>x.ProductPrice)
                        .Where(x=>x.IsActive==true
                        && !productSkuIDs.Contains(x.Id))
                        .OrderByDescending(x=>x.CreatedDate)
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
            
            foreach(var p in products)
            {
                var productPromote = await _context.ProductPromotion
                                                .Where(x => x.ProductId == p.Id)
                                                .FirstOrDefaultAsync();

                var data= new GetLandingProductLatestResponse(){
                    ProductId=p.Id,
                    Url=p.ProductImage.Where(x=>x.isMain==true).Select(x=>x.Url).SingleOrDefault(),
                    Name=p.Name,
                    OriginalPrice=p.ProductPrice.Select(x=>x.Price).SingleOrDefault(),
                    CreatedDate=p.CreatedDate,
                };

                if (productPromote != null)
                {
                    data.PromotePrice = productPromote.TotalAmt;
                    data.PromotePercent = productPromote.Percent;
                }
                else
                {
                    data.PromotePrice = 0;
                    data.PromotePercent = 0;
                }

                response.Add(data);
            }

          return response;
            
        }
        public async Task<List<GetProductByRelatedCategryResponse>> GetProductByRelatedCategry(GetProductByRelatedCategryRequest request)
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

            List<GetProductByRelatedCategryResponse> response=new List<GetProductByRelatedCategryResponse>();

            var categoryIDs= await _context.ProductCategory
                            .Where(x=>x.SubCategoryId==request.CategoryId)
                            .Select(x=>x.Id).ToListAsync();
            var products= await _context.Product
                        .Include(x=>x.ProductImage)
                        .Include(x=>x.ProductPrice)
                        .Where(x=>categoryIDs.Contains(x.ProductCategoryId)
                        && x.IsActive==true
                        && !productSkuIDs.Contains(x.Id))
                        .OrderByDescending(x=>x.CreatedDate)
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
            
            foreach(var p in products)
            {
                var productPromote = await _context.ProductPromotion
                                                .Where(x => x.ProductId == p.Id)
                                                .FirstOrDefaultAsync();

                var data= new GetProductByRelatedCategryResponse(){
                    ProductId=p.Id,
                    Url=p.ProductImage.Where(x=>x.isMain==true).Select(x=>x.Url).SingleOrDefault(),
                    Name=p.Name,
                    OriginalPrice=p.ProductPrice.Select(x=>x.Price).SingleOrDefault(),                    
                    ProductCategoryId=p.ProductCategoryId,
                };

                if (productPromote != null)
                {
                    data.PromotePrice = productPromote.TotalAmt;
                    data.PromotePercent = productPromote.Percent;
                }
                else
                {
                    data.PromotePrice = 0;
                    data.PromotePercent = 0;
                }


                response.Add(data);
            }

          return response.OrderByDescending(x=>x.PromotePrice).ToList();
        }
        public async Task<List<GetProductByRelatedTagResponse>> GetProductByRelatedTag(GetProductByRelatedTagRequest request)
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

            List<GetProductByRelatedTagResponse> response=new List<GetProductByRelatedTagResponse>();
            
            int[] productTags= await _context.ProductTag
                            .Where(x=>request.TagIDs
                            .Contains(x.TagId))
                            .Select(x=>x.ProductId)
                            .ToArrayAsync();

            var products= await _context.Product
                        .Where(x=>productTags.Contains(x.Id)
                        && x.IsActive==true
                        && !productSkuIDs.Contains(x.Id))
                        .Include(x=>x.ProductImage)
                        .Include(x=>x.ProductPrice)                        
                        .OrderByDescending(x=>x.CreatedDate)
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
            
            foreach(var p in products)
            {
                var productPromote = await _context.ProductPromotion
                                                .Where(x => x.ProductId == p.Id)
                                                .FirstOrDefaultAsync();

                var data= new GetProductByRelatedTagResponse(){
                    ProductId=p.Id,
                    Url=p.ProductImage.Where(x=>x.isMain==true).Select(x=>x.Url).SingleOrDefault(),
                    Name=p.Name,
                    OriginalPrice=p.ProductPrice.Select(x=>x.Price).SingleOrDefault(),                    
                    TagId=_context.ProductTag.Where(x=>x.ProductId==p.Id).Select(x=>x.TagId).ToArray(),
                };

                if (productPromote != null)
                {
                    data.PromotePrice = productPromote.TotalAmt;
                    data.PromotePercent = productPromote.Percent;
                }
                else
                {
                    data.PromotePrice = 0;
                    data.PromotePercent = 0;
                }

                response.Add(data);
            }

          return response.OrderByDescending(x=>x.PromotePrice).ToList();
        }
        public async Task<GetLandingProductCategoryResponse> GetLandingProductCategory(GetLandingProductCategoryRequest request)
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

            GetLandingProductCategoryResponse res=new GetLandingProductCategoryResponse();
                
            var subCategoryIDs= await  _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true
                        && x.SubCategoryId==request.ProductCategoryId)
                        .Select(x=>x.Id)
                        .ToListAsync();

            var category= await _context.ProductCategory.Where(x=>x.Id==request.ProductCategoryId).SingleOrDefaultAsync();
            res.MainCategoryId=category.Id;
            res.MainCategoryName=category.Name;
            res.Url=category.Url;

            var products= await _context.Product
                .Include(x=>x.ProductImage)
                .Include(x=>x.ProductPrice)
                .Include(x=>x.ProductCategory)
                .Where(x=>subCategoryIDs.Contains(x.ProductCategoryId)
                && x.IsActive==true
                && !productSkuIDs.Contains(x.Id))
                .OrderByDescending(x=>x.CreatedDate)
                .Skip((request.PageNumber-1)*request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var LandingProductCategory=new List<LandingProductCategory>();
    
            foreach(var p in products)
            {
            var productPromote = await _context.ProductPromotion
                                        .Where(x => x.ProductId == p.Id)
                                        .FirstOrDefaultAsync();

            var data= new LandingProductCategory(){
                    ProductId=p.Id,
                    Url=p.ProductImage.Where(x=>x.isMain==true).Select(x=>x.Url).SingleOrDefault(),
                    Name=p.Name,
                    OriginalPrice=p.ProductPrice.Select(x=>x.Price).SingleOrDefault(),
                    SubCategoryId=p.ProductCategoryId,
                    SubCategoryName=p.ProductCategory.Name                            
                };

            if (productPromote != null)
            {
                data.PromotePrice = productPromote.TotalAmt;
                data.PromotePercent = productPromote.Percent;
            }
            else
            {
                data.PromotePrice = 0;
                data.PromotePercent = 0;
            }
            LandingProductCategory.Add(data);
            }
        res.LandingProductCategory=LandingProductCategory;

        return res;
        }
        public async Task<List<GetProductListResponse>> GetProductList(GetProductListRequest request)
        {
            List<GetProductListResponse> response = new List<GetProductListResponse>();
            var productList=new List<Product>();
            int totalCount=0;

            int[] pTags={};
            if(request.TagIDs!=null && request.TagIDs.Length>0)
            {
                pTags=await _context.ProductTag
                    .Where(x=>request.TagIDs.Contains(x.TagId))
                    .Select(x=>x.ProductId).ToArrayAsync();
                if(pTags.Count()==0)
                {
                    return null;
                }
            }

            int[] pQty={};
            if(request.Count>0)
            {                
                pQty=await (from sku in _context.ProductSku
                                group sku by sku.ProductId into newSku
                                select new
                                {
                                ProductId = newSku.Key,
                                TotalQty = newSku.Sum(x => x.Qty), 
                                })
                                .Where(x=>x.TotalQty>=request.Count)
                                .Select(x=>x.ProductId)
                                .ToArrayAsync();
                 if(pQty.Count()==0)
                {
                    return null;
                }
            }

            if(request.Filter==1)// All
            {
                productList= await _context.Product
                            .Where(x => x.IsActive == true
                            && (String.IsNullOrEmpty(request.SearchText) || x.Name.Contains(request.SearchText))
                            && (request.ProductCategoryId==0 || x.ProductCategoryId==request.ProductCategoryId)
                            && (pTags.Length==0 || pTags.Contains(x.Id))
                            && (pQty.Length==0 || pQty.Contains(x.Id))
                            )
                            .OrderByDescending(y => y.CreatedDate)
                            .Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize)
                            .ToListAsync();
                totalCount=await _context.Product
                            .Where(x => x.IsActive == true
                            && (String.IsNullOrEmpty(request.SearchText) || x.Name.Contains(request.SearchText))
                            && (request.ProductCategoryId==0 || x.ProductCategoryId==request.ProductCategoryId)
                            && (pTags.Length==0 || pTags.Contains(x.Id))
                            && (pQty.Length==0 || pQty.Contains(x.Id))
                            )
                            .OrderByDescending(y => y.CreatedDate)
                            .CountAsync();
            }
            else if (request.Filter==2) // Promotion
            {
                var ppIDs = await _context.ProductPromotion                            
                            .Select(x=>x.ProductId)
                            .ToListAsync();
                productList= await _context.Product
                            .Where(x => x.IsActive == true
                            && ppIDs.Contains(x.Id)
                            && (String.IsNullOrEmpty(request.SearchText) 
                            || x.Name.Contains(request.SearchText))
                            && (request.ProductCategoryId==0 || x.ProductCategoryId==request.ProductCategoryId)
                            && (pTags.Length==0 || pTags.Contains(x.Id))
                            && (pQty.Length==0 || pQty.Contains(x.Id)))
                            .OrderByDescending(y => y.CreatedDate)
                            .Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize)
                            .ToListAsync();
                
                totalCount=await _context.Product
                            .Where(x => x.IsActive == true
                            && ppIDs.Contains(x.Id)
                            && (String.IsNullOrEmpty(request.SearchText) 
                            || x.Name.Contains(request.SearchText))
                            && (request.ProductCategoryId==0 || x.ProductCategoryId==request.ProductCategoryId)
                            && (pTags.Length==0 || pTags.Contains(x.Id))
                            && (pQty.Length==0 || pQty.Contains(x.Id)))
                            .OrderByDescending(y => y.CreatedDate)
                            .CountAsync();
            }
            else if(request.Filter==3){ // Out of stock

                int[] outOfStock =await (from sku in _context.ProductSku
                                group sku by sku.ProductId into newSku
                                select new
                                {
                                ProductId = newSku.Key,
                                TotalQty = newSku.Sum(x => x.Qty), 
                                })
                                .Where(x=>x.TotalQty<=0)
                                .Select(x=>x.ProductId)
                                .ToArrayAsync();

                productList= await _context.Product
                            .Where(x => x.IsActive == true
                            && outOfStock.Contains(x.Id)
                            && (String.IsNullOrEmpty(request.SearchText) 
                            || x.Name.Contains(request.SearchText))
                            && (request.ProductCategoryId==0 || x.ProductCategoryId==request.ProductCategoryId)
                            && (pTags.Length==0 || pTags.Contains(x.Id))
                            && (pQty.Length==0 || pQty.Contains(x.Id)))
                            .OrderByDescending(y => y.CreatedDate)
                            .Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize)
                            .ToListAsync();

                totalCount= await _context.Product
                            .Where(x => x.IsActive == true
                            && outOfStock.Contains(x.Id)
                            && (String.IsNullOrEmpty(request.SearchText) 
                            || x.Name.Contains(request.SearchText))
                            && (request.ProductCategoryId==0 || x.ProductCategoryId==request.ProductCategoryId)
                            && (pTags.Length==0 || pTags.Contains(x.Id))
                            && (pQty.Length==0 || pQty.Contains(x.Id)))
                            .OrderByDescending(y => y.CreatedDate)
                            .CountAsync();
            }
            else { // Reward
                var proIDs = await _context.ProductReward
                            .Select(x=>x.ProductId)
                            .ToListAsync();
                productList= await _context.Product
                            .Where(x => x.IsActive == true
                            && proIDs.Contains(x.Id)
                            && (String.IsNullOrEmpty(request.SearchText) 
                            || x.Name.Contains(request.SearchText))
                            && (request.ProductCategoryId==0 || x.ProductCategoryId==request.ProductCategoryId)
                             && (pTags.Length==0 || pTags.Contains(x.Id))
                            && (pQty.Length==0 || pQty.Contains(x.Id)))
                            .OrderByDescending(y => y.CreatedDate)
                            .Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize)
                            .ToListAsync();

                totalCount= await _context.Product
                            .Where(x => x.IsActive == true
                            && proIDs.Contains(x.Id)
                            && (String.IsNullOrEmpty(request.SearchText) 
                            || x.Name.Contains(request.SearchText))
                            && (request.ProductCategoryId==0 || x.ProductCategoryId==request.ProductCategoryId)
                             && (pTags.Length==0 || pTags.Contains(x.Id))
                            && (pQty.Length==0 || pQty.Contains(x.Id)))
                            .OrderByDescending(y => y.CreatedDate)
                            .CountAsync();
            }

            if (productList != null)
            {
                foreach (var product in productList)
                {
                    var productPromote= await _context.ProductPromotion
                                                .Where(x => x.ProductId == product.Id)
                                                .FirstOrDefaultAsync();

                    GetProductListResponse productListRes = new GetProductListResponse();
                    productListRes.Count = totalCount;
                    productListRes.Id = product.Id;
                    productListRes.Name = product.Name;
                    productListRes.Url = await _context.ProductImage.Where(x => x.ProductId == product.Id && x.isMain==true).Select(x => x.Url).SingleOrDefaultAsync();
                    productListRes.OriginalPrice= await _context.ProductPrice
                                                .Where(x => x.ProductId == product.Id && x.isActive == true)
                                                .OrderByDescending(x => x.StartDate)
                                                .Select(x=>x.Price)
                                                .FirstOrDefaultAsync();

                    if(productPromote!=null)
                    {
                        productListRes.PromotePrice = productPromote.TotalAmt;
                        productListRes.PromotePercent = productPromote.Percent;
                    }
                    else
                    {
                        productListRes.PromotePrice = 0;
                        productListRes.PromotePercent = 0;
                    }

                    var productSkuList = await _context.ProductSku.Where(x => x.ProductId == product.Id).ToListAsync();
                    if (productSkuList.Count > 0)
                    {
                        foreach (var item in productSkuList)
                        {
                            productListRes.Qty += item.Qty;
                            var skuKeys = await _context.ProductSkuValue
                                .Where(x => x.ProductId == item.ProductId && x.SkuId == item.SkuId)
                                .ToListAsync();

                            var skuValue = await(from psku in _context.ProductSkuValue
                                                 from pvopt in _context.ProductVariantOption
                                                 where psku.ProductId == item.ProductId
                                                 && psku.SkuId == item.SkuId
                                                 && psku.ProductId == pvopt.ProductId
                                                 && psku.VariantId == pvopt.VariantId
                                                 && psku.ValueId == pvopt.ValueId
                                                 select pvopt.ValueName).ToListAsync();
                            productListRes.Sku += string.Join(",", skuValue) + " ";
                        }
                    }
                    else
                    {
                        productListRes.Qty = 0;
                        productListRes.Sku = "";
                    }

                    #region  product reward
                    var productReward=await _context.ProductReward
                                        .Where(x=>x.ProductId==product.Id)
                                        .SingleOrDefaultAsync();
                    if(productReward!=null)
                    {
                        productListRes.RewardAmount=productReward.RewardAmount;
                        productListRes.FixedAmount=productReward.FixedAmount;
                        productListRes.Point=productReward.Point;
                        productListRes.RewardPercent=productReward.RewardPercent;
                    }
                    else{
                        productListRes.RewardAmount=0;
                        productListRes.FixedAmount=0;
                        productListRes.Point=0;
                        productListRes.RewardPercent=0;
                    }
                    #endregion

                    response.Add(productListRes);
                }
            }
            return response;
        }
        public async Task<ResponseStatus> DeleteProduct(DeleteProductRequest request)
        {
            ResponseStatus response = new ResponseStatus();

            Product product = await _context.Product.Where(x => x.Id == request.ProductId).FirstOrDefaultAsync();
            if (product == null)
            {
                response.StatusCode = StatusCodes.Status401Unauthorized;
                response.Message = "Invalid product Id.";
                return response;
            }
            else
            {
                // var orderList = await _context.OrderDetail.Where(x => x.ProductId == request.ProductId).FirstOrDefaultAsync();
                // if (orderList != null)
                // {
                //     response.Message = "ဝယ်သူမှ ပစ္စည်းမှာယူထားသောကြောင့် ဤကုန်ပစ္စည်းကို ပယ်ဖျက်၍ မရပါ";
                //     response.StatusCode = StatusCodes.Status400BadRequest;
                //     return response;
                // }
                // else
                // {
                    product.IsActive = false;
                    try
                    {
                        await _context.SaveChangesAsync();
                        response.StatusCode = StatusCodes.Status200OK;
                        response.Message = "အောင်မြင်စွာ ပယ်ဖျက်ပြီးပါပြီ။";
                        return response;
                    }
                    catch (Exception ex)
                    {
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        response.Message = ex.Message;
                        return response;
                    }
                // }
            }
        }
        public async Task<ProductSearchResponse> ProductSearch(ProductSearchRequest request,int userId,int platform)
        {
            #region  Activity Log
            if(!string.IsNullOrEmpty(request.ProductName))
            {
                 ActivityLog data=new ActivityLog(){
                UserId=userId,
                ActivityTypeId=MayMayShopConst.ACTIVITY_TYPE_SEARCH,
                Value=request.ProductName,
                CreatedBy=userId,
                CreatedDate=DateTime.Now,
                PlatformId=platform,
                ResultCount=_context.Product.Where(x=>x.Name.Contains(request.ProductName)).Count()
            };
            _context.ActivityLog.Add(data);
            await _context.SaveChangesAsync();

            // Keyword
            var keyworkList=request.ProductName.ToLower().Split(" ");
            foreach(var key in keyworkList)
            {
                var exitKeyword= await _context.SearchKeyword
                                .Where(x=>x.Name.ToLower()==key.ToLower())
                                .SingleOrDefaultAsync();
                if(exitKeyword==null && !MayMayShopConst.NonKeyword.Contains(key.ToLower()))//new keyword
                {
                    var newKeyword=new SearchKeyword(){
                        Name=key.ToLower(),
                        CreatedBy=userId,
                        CreatedDate=DateTime.Now
                    };
                    _context.SearchKeyword.Add(newKeyword);
                    await _context.SaveChangesAsync();

                    var newKeywordTrn=new SearchKeywordTrns(){
                        SearchKeywordId=newKeyword.Id,
                        Count=1,
                        CreatedDate=DateTime.Now,
                    };
                    _context.SearchKeywordTrns.Add(newKeywordTrn);
                    await _context.SaveChangesAsync();
                }
                else{
                    var searchTrn=await _context.SearchKeywordTrns
                                .Where(x=>x.SearchKeywordId==exitKeyword.Id
                                && x.CreatedDate.Date==DateTime.Now.Date)
                                .ToListAsync();
                    if (searchTrn.Count()==0)
                    {
                        var newKeywordTrn=new SearchKeywordTrns(){
                        SearchKeywordId=exitKeyword.Id,
                        Count=1,
                        CreatedDate=DateTime.Now,
                        };
                    _context.SearchKeywordTrns.Add(newKeywordTrn);
                    }
                    else{
                        searchTrn.FirstOrDefault().Count=searchTrn.Count+1;
                    }
                    await _context.SaveChangesAsync();
                }

            }

            }
            #endregion

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

            ProductSearchResponse response = new ProductSearchResponse();

            var productList=new List<ProductInfo>();

            #region Search by Name
            if(request.SearchType==MayMayShopConst.SEARCHTYPE_NAME)
            {
                productList = await ( (
                                        from p in _context.Product
                                        where p.IsActive == true
                                        &&    (String.IsNullOrEmpty(request.ProductName) || p.Name.Contains(request.ProductName))
                                        && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(p.Id))
                                        select new ProductInfo{
                                            Id = p.Id,
                                            Name = p.Name,
                                            OriginalPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                            PromotePrice = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.TotalAmt).FirstOrDefault(),
                                            PromotePercent=_context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.Percent).FirstOrDefault(),
                                            CreatedDate = p.CreatedDate,
                                            OrderCount = _context.OrderDetail.Where(x=>x.ProductId==p.Id).Sum(x => x.Qty),                                            
                                            Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=> x.ThumbnailUrl).SingleOrDefault()
                                        })
            )            
            .ToListAsync();
            }
            #endregion

            #region Search by MainCategory
            else if(request.SearchType==MayMayShopConst.SEARCHTYPE_CATEGORY)
            {
                var categoryIDs= await _context.ProductCategory.Where(x=>x.SubCategoryId==request.ProductCategoryId).Select(x=>x.Id).ToListAsync();

                productList = await ( (
                                        from p in _context.Product
                                        where p.IsActive == true
                                        && (categoryIDs.Count()==0 || categoryIDs.Contains(p.ProductCategoryId))
                                        && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(p.Id))
                                        select new ProductInfo{
                                            Id = p.Id,
                                            Name = p.Name,
                                            OriginalPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                            PromotePrice = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.TotalAmt).FirstOrDefault(),
                                            PromotePercent=_context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.Percent).FirstOrDefault(),
                                            CreatedDate = p.CreatedDate,
                                            OrderCount = _context.OrderDetail.Where(x=>x.ProductId==p.Id).Sum(x => x.Qty),
                                            Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                                        })
            )
            .ToListAsync();
            }
            #endregion

            #region Search by Tag
            else if(request.SearchType==MayMayShopConst.SEARCHTYPE_TAG)
            {           
                var proIDs=await _context.ProductTag
                            .Where(x=>request.tagIds.Count() == 0 || request.tagIds.Contains(x.TagId))
                            .Select(x=>x.ProductId)
                            .ToListAsync();

                productList = await ( (
                                        from p in _context.Product                                       
                                        where p.IsActive == true
                                        &&    (proIDs.Count() == 0 ||proIDs.Contains(p.Id))
                                        && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(p.Id))
                                        select new ProductInfo{
                                            Id = p.Id,
                                            Name = p.Name,
                                            OriginalPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                            PromotePrice = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.TotalAmt).FirstOrDefault(),
                                            PromotePercent=_context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.Percent).FirstOrDefault(),
                                            CreatedDate = p.CreatedDate,
                                            OrderCount = _context.OrderDetail.Where(x=>x.ProductId==p.Id).Sum(x => x.Qty),
                                            Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                                        })
            )
            .ToListAsync();
            }
            #endregion

            #region Search by New Arrived Product
            else if(request.SearchType==MayMayShopConst.SEARCHTYPE_LATEST)
            {
                productList = await ( (
                                        from p in _context.Product
                                        where p.IsActive == true
                                        && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(p.Id))
                                        orderby p.CreatedDate descending
                                        select new ProductInfo{
                                            Id = p.Id,
                                            Name = p.Name,
                                            OriginalPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                            PromotePrice = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.TotalAmt).FirstOrDefault(),
                                            PromotePercent=_context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.Percent).FirstOrDefault(),
                                            CreatedDate = p.CreatedDate,
                                            OrderCount = _context.OrderDetail.Where(x=>x.ProductId==p.Id).Sum(x => x.Qty),
                                            Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                                        })
            )
            .ToListAsync();
            }
            #endregion

            #region Search by Promotion Product
            else if(request.SearchType==MayMayShopConst.SEARCHTYPE_PROMOTION)
            {                
                var proIDs=await _context.ProductPromotion
                                .Where(x=>(productSkuIDs.Count()==0 || !productSkuIDs.Contains(x.ProductId)))                                
                                .Select(x=>x.ProductId)
                                .ToListAsync();

                productList = await ( (
                                        from p in _context.Product
                                        where p.IsActive == true
                                        && (proIDs.Count()==0 || proIDs.Contains(p.Id))                        
                                        select new ProductInfo{
                                            Id = p.Id,
                                            Name = p.Name,
                                            OriginalPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                            PromotePrice = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.TotalAmt).FirstOrDefault(),
                                            PromotePercent=_context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.Percent).FirstOrDefault(),
                                            CreatedDate = p.CreatedDate,
                                            OrderCount = _context.OrderDetail.Where(x=>x.ProductId==p.Id).Sum(x=>x.Qty),
                                            Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                                        })
            )
            .ToListAsync();
            }
            #endregion
            
            #region Search by SubCategory
            else if(request.SearchType==MayMayShopConst.SEARCHTYPE_SUB_CATEGORY)
            {
                productList = await ((
                                        from p in _context.Product
                                        where p.IsActive == true
                                        && (request.ProductCategoryId==0 || p.ProductCategoryId==request.ProductCategoryId)
                                        && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(p.Id))        
                                        select new ProductInfo{
                                            Id = p.Id,
                                            Name = p.Name,
                                            OriginalPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                            PromotePrice = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.TotalAmt).FirstOrDefault(),
                                            PromotePercent=_context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.Percent).FirstOrDefault(),
                                            CreatedDate = p.CreatedDate,
                                            OrderCount = _context.OrderDetail.Where(x=>x.ProductId==p.Id).Sum(x => x.Qty),
                                            Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                                        })
            )
            .ToListAsync();
            }
            #endregion
            
            #region Search by Best Seller Product
            else if(request.SearchType==MayMayShopConst.SEARCHTYPE_BEST_SELLER)
            {
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

                productList = await ( (
                                        from p in _context.Product
                                        .Where(x => x.IsActive == true 
                                        && productListIDS.Contains(x.Id)
                                        && (productSkuIDs.Count()==0 || !productSkuIDs.Contains(x.Id)))
                                        select new ProductInfo{
                                            Id = p.Id,
                                            Name = p.Name,
                                            OriginalPrice = _context.ProductPrice.Where(x => x.ProductId == p.Id && x.isActive == true).Select(x => x.Price).FirstOrDefault(),
                                            PromotePrice = _context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.TotalAmt).FirstOrDefault(),
                                            PromotePercent=_context.ProductPromotion.Where(x => x.ProductId == p.Id).Select(x => x.Percent).FirstOrDefault(),
                                            CreatedDate = p.CreatedDate,
                                            OrderCount = _context.OrderDetail.Where(x=>x.ProductId==p.Id).Sum(x=>x.Qty),
                                            Url = _context.ProductImage.Where(x => x.ProductId==p.Id && x.isMain==true).Select(x=>x.Url).SingleOrDefault()
                                        })
            )
            .ToListAsync();
            }
            #endregion
            

            if (productList.Count > 0)
            {
                response.ProductList = productList;
                response.Count = productList.Count();
            }

            if (request.Choose != 0 && response.ProductList!=null)
            {
                if(request.SearchType==MayMayShopConst.SEARCHTYPE_PROMOTION)
                {
                if (request.Choose == 1)//"PriceLowToHigh"
                {
                    response.ProductList = response.ProductList.OrderBy(x => x.PromotePrice).ToList();
                }
                else if (request.Choose == 2)//"PriceHighToLow"
                {
                    response.ProductList = response.ProductList.OrderByDescending(x => x.PromotePrice).ToList();
                }
                else if (request.Choose == 3)//"LatestProduct"
                {
                    response.ProductList = response.ProductList.OrderByDescending(x => x.CreatedDate).ToList();
                }
                else if (request.Choose == 4)//"SellingProduct"
                {
                    response.ProductList = response.ProductList.OrderByDescending(x => x.OrderCount).ToList();
                }
                }
                else{
                    if (request.Choose == 1)//"PriceLowToHigh"
                {
                    response.ProductList = response.ProductList.OrderBy(x => x.OriginalPrice).ToList();
                }
                else if (request.Choose == 2)//"PriceHighToLow"
                {
                    response.ProductList = response.ProductList.OrderByDescending(x => x.OriginalPrice).ToList();
                }
                else if (request.Choose == 3)//"LatestProduct"
                {
                    response.ProductList = response.ProductList.OrderByDescending(x => x.CreatedDate).ToList();
                }
                else if (request.Choose == 4)//"SellingProduct"
                {
                    response.ProductList = response.ProductList.OrderByDescending(x => x.OrderCount).ToList();
                }
                }
                response.ProductList=response.ProductList.Skip((request.PageNumber-1)*request.PageSize).Take(request.PageSize).ToList();
                
            }

            return response;
        }
        public async Task<List<GetVariantByCategoryResponse>> GetVariantByCategoryId(int categoryId)
        {

            List<GetVariantByCategoryResponse> variants = await _context.Variant
                                                                .Where(x => x.ProductCategoryId == categoryId && x.IsDeleted == false)
                                                                .Select(x => new GetVariantByCategoryResponse{
                                                                    Id = x.Id,
                                                                    Name = x.Name
                                                                } ) .ToListAsync();
            return variants;
        }
        public async Task ProductSkuHold(ProductSkuHoldRequest req)
        {
            foreach (var item in req.Sku)
            {
                var sku = await _context
                    .TrnProductSku.Where(x => x.SkuId == item.SkuId && x.ProductId == req.ProductId).FirstOrDefaultAsync();
                sku.Qty = item.Qty;

                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<GetProductSkuResponse>> GetProductSku(GetProductSkuRequest req)
        {
            var resp = new List<GetProductSkuResponse>();
            var skuList = await((
                                from psku   in _context.TrnProductSku
                                join skuval in _context.TrnProductSkuValue
                                on new { psku.ProductId, psku.SkuId } equals new {skuval.ProductId, skuval.SkuId}
                                join vari   in _context.TrnProductVariantOption
                                on new { skuval.ProductId, skuval.ValueId, skuval.VariantId} equals new {vari.ProductId, vari.ValueId, vari.VariantId}
                                select new GetProductSkuResponse{
                                    ProductId = psku.ProductId,
                                    SkuId = psku.SkuId,
                                    VariantOptions =vari.ValueName,
                                    Qty = psku.Qty
                                }
            ).Distinct()
            ).ToListAsync();
            if (skuList.Count > 0)
            {
                resp = skuList;
            }

            return resp;
        }
        public async Task<GetProductVariantResponse> GetProductVariant(GetProductSkuRequest req)
        {
            var resp = new GetProductVariantResponse();
            var variants = await _context.TrnProductVariantOption
                            .Where(x => x.ProductId == req.ProductId)
                            .Select(x => new ValueList {
                                ValueName = x.ValueName,
                                ValueId = x.ValueId,
                                VariantId = x.VariantId
                            }).ToListAsync();
            variants = variants.GroupBy(x => x.VariantId).Select(x =>x.First()).ToList();

            if (variants.Count > 0)
            {
                for (int i = 0; i < variants.Count; i++)
                {
                    var valueList = await _context.TrnProductVariantOption
                            .Where(x => x.ProductId == req.ProductId && x.VariantId == variants[i].VariantId)
                            .Select(x => new ValueList {
                                ValueName = x.ValueName,
                                ValueId = x.ValueId,
                                VariantId = x.VariantId
                            }).ToListAsync();

                    if (i == 0)
                    {
                        resp.ValueList1 = valueList;
                    }
                    else if (i == 1)
                    {
                        resp.ValueList2 = valueList;
                    }
                    else if (i == 2)
                    {
                        resp.ValueList3 = valueList;
                    }
                }
            }
        return resp;
        }
        public async Task<List<GetProductNameSuggestionResponse>> GetProductNameSuggestion(GetProductNameSuggestionRequest request)
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
            var res = await _context.Product.Where(x=>x.Name.Contains(request.SearchText) 
                                                    && x.IsActive==true 
                                                    && !productSkuIDs.Contains(x.Id))
                                                    .OrderByDescending(x =>x.Name.StartsWith(request.SearchText))
                                                    .Select(x => new GetProductNameSuggestionResponse{
                                                        ImageUrl =  _context.ProductImage.Where(p=> p.ProductId == x.Id && p.isMain == true).Select(p => p.ThumbnailUrl).FirstOrDefault(),
                                                        Name = x.Name
                                                    })
                                                    .Skip((request.PageNumber-1)).Take(request.PageSize).ToListAsync();

            return res;
        }
        public async Task<List<GetBestSellingProductResponse>> GetBestSellingProduct(GetBestSellingProductRequest request)
        {
            var fromDate=DateTime.Today.AddDays(-MayMayShopConst.BEST_SELLER_DURATION);
            var toDate=DateTime.Now;

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

            List<int> orderListIDs = await _context.Order
                                    .Where(x => x.OrderDate.Date >= fromDate.Date 
                                    && x.OrderDate.Date <= toDate.Date)
                                    .Select(x=>x.Id)
                                    .Skip((request.PageNumber-1)*request.PageSize)
                                    .Take(request.PageSize)
                                    .ToListAsync();
            List<int> productListIDS = await _context.OrderDetail
                                    .Where(x=>orderListIDs.Contains(x.OrderId))
                                    .Select(x=>x.ProductId)
                                    .ToListAsync();                             
            List<Product> productList = await _context.Product
                                    .Where(x => x.IsActive == true 
                                    && productListIDS.Contains(x.Id)
                                    && !productSkuIDs.Contains(x.Id))
                                    .ToListAsync();
            
            List<GetBestSellingProductResponse> response = new List<GetBestSellingProductResponse>();
            
            if (productList != null)
            {
                foreach (var product in productList)
                {
                    GetBestSellingProductResponse orderListProduct = new GetBestSellingProductResponse();
                    orderListProduct.Name = product.Name;
                    orderListProduct.OrderCount = _context.OrderDetail.Where(x=>x.ProductId==product.Id && productListIDS.Contains(x.ProductId)).Count();                   
                    orderListProduct.Id = product.Id;
                    var productImage = await _context.ProductImage.Where(x => x.ProductId == product.Id && x.isMain == true).FirstOrDefaultAsync();
                    if (productImage != null)
                    {
                        orderListProduct.Url = productImage.Url;
                    }
                    else
                    {
                        orderListProduct.Url = "";
                    }
                    var price = await _context.ProductPrice.OrderByDescending(y => y.StartDate).Where(x => x.ProductId == product.Id).FirstOrDefaultAsync();

                    if (price != null)
                    {
                        orderListProduct.OriginalPrice = price.Price;
                    }
                    else
                    {
                        orderListProduct.OriginalPrice = 0;
                    }

                    var productPromote = await _context.ProductPromotion
                                            .Where(x => x.ProductId == product.Id)
                                            .FirstOrDefaultAsync();
                    if (productPromote != null)
                    {
                        orderListProduct.PromotePrice = productPromote.TotalAmt;
                        orderListProduct.PromotePercent = productPromote.Percent;
                    }
                    else
                    {
                        orderListProduct.PromotePrice = 0;
                        orderListProduct.PromotePercent = 0;
                    }

                    response.Add(orderListProduct);
                }
            }
            response = response.OrderByDescending(x => x.OrderCount).ToList();            
            return response;
        }
        public async Task<ResponseStatus> UploadProduct(UploadProductRequest request,int currentLoginID)
        {
            ResponseStatus response=new ResponseStatus();

            using (MemoryStream excelMemoryStream = new MemoryStream())
            {
                await request.File.CopyToAsync(excelMemoryStream);
                DataTable excelProduct =_MayMayShopServices.ToDataTable(excelMemoryStream, "Product");
                
                #region Check excel template format
                if(excelProduct.Columns.Count!=17)
                {
                    return new ResponseStatus(){
                        StatusCode=StatusCodes.Status400BadRequest,
                        Message="Column count doesn't match in excel template!."
                    };
                }
                if(excelProduct.Rows.Count<=0)
                {
                    return new ResponseStatus(){
                        StatusCode=StatusCodes.Status400BadRequest,
                        Message="There's no data to import!"
                    };
                }
                for(int i=0;i<excelProduct.Rows.Count;i++)
                {
                    var existCategory= _context.ProductCategory.Where(x=>x.Name==excelProduct.Rows[i]["SubCategory"].ToString() && x.SubCategoryId!=null).FirstOrDefault();
                    if(existCategory==null)
                    {
                        return new ResponseStatus(){
                        StatusCode=StatusCodes.Status400BadRequest,
                        Message=string.Format("{0} does not exist in out system, plz upload category first!",excelProduct.Rows[i]["SubCategory"].ToString())
                    };
                    }
                    // assume column 10=1 varinat, 12=2 variant, 14=3 variant
                    string v1=excelProduct.Rows[i]["Variant1"].ToString();
                    string v2=excelProduct.Rows[i]["Variant2"].ToString();
                    string v3=excelProduct.Rows[i]["Variant3"].ToString();

                    if(v1!="#Empty#" && !_context.Variant.Any(x=>x.ProductCategoryId==existCategory.Id && x.Name==v1))
                    {
                        return new ResponseStatus(){
                        StatusCode=StatusCodes.Status400BadRequest,
                        Message=string.Format("{0} category doesn't have {1} variant!",excelProduct.Rows[i]["SubCategory"].ToString(),v1)
                    };
                    }
                    if(v2!="#Empty#" && !_context.Variant.Any(x=>x.ProductCategoryId==existCategory.Id && x.Name==v2))
                    {
                        return new ResponseStatus(){
                        StatusCode=StatusCodes.Status400BadRequest,
                        Message=string.Format("{0} category doesn't have {1} variant!",excelProduct.Rows[i]["SubCategory"].ToString(),v2)
                    };
                    }
                    if(v3!="#Empty#" && !_context.Variant.Any(x=>x.ProductCategoryId==existCategory.Id && x.Name==v3))
                    {
                        return new ResponseStatus(){
                        StatusCode=StatusCodes.Status400BadRequest,
                        Message=string.Format("{0} category doesn't have {1} variant!",excelProduct.Rows[i]["SubCategory"].ToString(),v3)
                    };
                    } 
                }   
               
                #endregion

                var imageList=_MayMayShopServices.ExcelPicture(excelMemoryStream, "Product",excelProduct.Rows.Count);            

                for(int i=0;i<excelProduct.Rows.Count;i++)
                {
                    #region Create ProductSku
                    var existCategory= _context.ProductCategory.Where(x=>x.Name==excelProduct.Rows[i]["SubCategory"].ToString() && x.SubCategoryId!=null).FirstOrDefault();
                    // assume column 10=1 varinat, 12=2 variant, 14=3 variant
                    string v1=excelProduct.Rows[i]["Variant1"].ToString();
                    string v2=excelProduct.Rows[i]["Variant2"].ToString();
                    string v3=excelProduct.Rows[i]["Variant3"].ToString();
                    string vValue1=excelProduct.Rows[i]["VariantValue1"].ToString();
                    string vValue2=excelProduct.Rows[i]["VariantValue2"].ToString();
                    string vValue3=excelProduct.Rows[i]["VariantValue3"].ToString();
                    
                    List<Options> Options=new List<Options>();

                    if(v1!="#Empty#")
                    {
                        var varinat=_context.Variant.Where(x=>x.ProductCategoryId==existCategory.Id && x.Name==v1).SingleOrDefault();
                        Options opt=new Options(){
                            VariantId=varinat.Id,
                            OptionValue=vValue1.Split(',').ToList()
                        };
                        Options.Add(opt);
                    }
                    if(v2!="#Empty#")
                    {
                        var varinat=_context.Variant.Where(x=>x.ProductCategoryId==existCategory.Id && x.Name==v2).SingleOrDefault();
                        Options opt=new Options(){
                            VariantId=varinat.Id,
                            OptionValue=vValue2.Split(',').ToList()
                        };
                        Options.Add(opt);
                    }
                    if(v3!="#Empty#")
                    {
                        var varinat=_context.Variant.Where(x=>x.ProductCategoryId==existCategory.Id && x.Name==v3).SingleOrDefault();
                        Options opt=new Options(){
                            VariantId=varinat.Id,
                            OptionValue=vValue3.Split(',').ToList()
                        };
                        Options.Add(opt);
                    }
                    
                    var trnProductFromRepo = await CreateDemoProduct(existCategory.Id);
                    PopulateSkuRequest populateSkuRequest=new PopulateSkuRequest()
                    {
                        ProductCategoryId=existCategory.Id,
                        Options=Options
                    };

                    var populateSkuResponse=await PopulateSku(populateSkuRequest,trnProductFromRepo.Id);
                    #endregion
                    
                    #region  Create Product

                    #region TagReq
                    var tagList=new List<Tag>();
                    foreach(var tag in excelProduct.Rows[i]["Tag"].ToString().Split(','))
                    {
                        var existTag=_context.Tag.Where(x=>x.Name==tag).SingleOrDefault();
                        if(existTag==null)//new tag
                        {
                            var newTag=new Tag(){
                                Id=0,
                                Name=tag,
                            };
                            tagList.Add(newTag);
                        }
                        else{ // exist tag
                             tagList.Add(existTag);
                        }
                    }
                    #endregion

                    #region  Sku req
                    //get product sku
                    
                    List<Sku> SkuList=new List<Sku>();
                    foreach(var sku in populateSkuResponse)
                    {
                        SkuList.Add(new Sku(){SkuId=sku.SkuId,Qty=int.Parse(excelProduct.Rows[i]["Quantity"].ToString())});
                    }
                    #endregion

                    #region  image req
                    List<ImageRequest> ImageList=new List<ImageRequest>();
                    UploadProductExcelImage uploadProductExcelImage=imageList.Where(x=>x.index==i+1).SingleOrDefault();
                    foreach(var img in uploadProductExcelImage.ExcelPicture)
                    {
                        ImageRequest newImg=new ImageRequest(){
                            ImageId=0,
                            ImageContent=_MayMayShopServices.ImageToBase64(img.Image,new System.Drawing.Imaging.ImageFormat(img.Image.RawFormat.Guid)),
                            Extension="png",
                            Action="New"
                        };
                        ImageList.Add(newImg);
                    } 
                    #endregion

                    CreateProductRequest productRequest=new CreateProductRequest(){
                        ProductId=populateSkuResponse.FirstOrDefault().ProductId,
                        Name=excelProduct.Rows[i]["ProductName"].ToString(),
                        Description=excelProduct.Rows[i]["Description"].ToString(),
                        Price=double.Parse(excelProduct.Rows[i]["Price"].ToString()),
                        Promotion=int.Parse(excelProduct.Rows[i]["Promotion(%)"].ToString()),
                        TagsList=tagList,
                        ProductClip=new ProductClipRequest(){
                                        Name=excelProduct.Rows[i]["VideoName"].ToString(),
                                        ClipPath=excelProduct.Rows[i]["VideoLink"].ToString(),
                                        SeqNo=i
                                     },
                        Sku=SkuList,
                        ImageList=ImageList,

                    };

                    var imgList = new List<ImageUrlResponse>();
                    foreach(var image in productRequest.ImageList)
                    {  
                        ImageUrlResponse img  =new ImageUrlResponse();                
                        img = await _MayMayShopServices.UploadToS3(image.ImageContent, image.Extension,MayMayShopConst.AWS_PRODUCT_PATH);
                        imgList.Add(img);
                    }
                
                    var res =await CreateProduct(productRequest,imgList, currentLoginID);
                    #endregion
                }                         
            }                       

            response.StatusCode=StatusCodes.Status200OK;
            return response;
        }
        public async Task<GetAllProductListBuyerResponse> GetAllProductListBuyer(GetAllProductListBuyerRequest request)
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

            GetAllProductListBuyerResponse response =new GetAllProductListBuyerResponse();
            var productCategory=await _context.ProductCategory
                        .Where(x=>x.IsDeleted!=true && (x.SubCategoryId==0 || string.IsNullOrEmpty(x.SubCategoryId.ToString())))
                        .Select(x=> new GetAllProductListMainCategoryBuyer
                        {Id=x.Id,
                        Name=x.Name,
                        Url=x.Url,
                        })
                        .ToListAsync();

            foreach(var mainCate in productCategory)
            {
                var subCategoryIDs= _context.ProductCategory
                                    .Where(x=>x.IsDeleted!=true
                                    && x.SubCategoryId==mainCate.Id)
                                    .Select(x=>x.Id)
                                    .ToList();                              

                var products= _context.Product
                            .Where(p=>subCategoryIDs.Contains(p.ProductCategoryId)
                            && p.IsActive==true
                            && !productSkuIDs.Contains(p.Id))
                            .Select(p=>new GetAllProductListBuyer{
                                ProductId=p.Id,
                                Url=_context.ProductImage.Where(img=>img.isMain==true && img.ProductId==p.Id).Select(img=>img.Url).SingleOrDefault(),
                                Name=p.Name,
                                OriginalPrice=_context.ProductPrice.Where(oPrice=>oPrice.ProductId==p.Id).Select(oPrice=>oPrice.Price).SingleOrDefault(),
                                PromotePrice=_context.ProductPromotion
                                                .Where(pPrice => pPrice.ProductId == p.Id)
                                                .Select(promotePrice=>promotePrice.TotalAmt)
                                                .FirstOrDefault(),
                                PromotePercent=_context.ProductPromotion
                                                .Where(pPercent => pPercent.ProductId == p.Id)
                                                .Select(promotePrice=>promotePrice.Percent)
                                                .FirstOrDefault(),
                            })
                            .OrderByDescending(p=>p.PromotePrice)
                            .Skip((request.PageNumber-1)*request.PageSize)
                            .Take(request.PageSize)
                            .ToList();

                    mainCate.ProductListBuyers=products;
            
            }
            response.MainCategory=productCategory;
            return response;
        }

        public async Task<List<ProductImage>> GetAllProductImageByProductId(int productId)
        {
            return await _context.ProductImage
                    .Where(x=>x.ProductId==productId)
                    .ToListAsync();
        }

        public async Task<GetProductByBrandResponse> GetProductByBrand(GetProductByBrandRequest request)
        {
            //product id that qty is 0 or less than 0.
            var productSkuIDs= await (from sku in _context.ProductSku
                                group sku by sku.ProductId into newSku
                                select new
                                {
                                ProductId = newSku.Key,
                                TotalQty = newSku.Sum(x => x.Qty), 
                                })
                                .Where(x=>x.TotalQty<=0)
                                .Select(x=>x.ProductId)
                                .ToArrayAsync();

            GetProductByBrandResponse resp = new GetProductByBrandResponse();
            var brand = await _context.Brand.Where(x =>x.Id == request.BrandId).SingleOrDefaultAsync();
            if (brand != null)
            {
                resp.BrandLogo = brand.LogoUrl;
                resp.BrandName = brand.Name;
                resp.Url = brand.Url;
            }

            var products= await _context.Product
                        .Include(x=>x.ProductImage)
                        .Include(x=>x.ProductPrice)
                        .Where(x=> x.BrandId == request.BrandId 
                                && x.IsActive==true
                                && !productSkuIDs.Contains(x.Id)
                            )
                        .OrderByDescending(x=>x.CreatedDate)
                        .Skip((request.PageNumber-1)*request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync();
            var productList = new List<BrandProduct>();
            
            foreach(var p in products)
            {
                var productPromote = await _context.ProductPromotion
                                                .Where(x => x.ProductId == p.Id)
                                                .FirstOrDefaultAsync();

                var data= new BrandProduct(){
                    ProductId=p.Id,
                    Url=p.ProductImage.Where(x=>x.isMain==true).Select(x=>x.Url).SingleOrDefault(),
                    Name=p.Name,
                    OriginalPrice=p.ProductPrice.Select(x=>x.Price).SingleOrDefault(),
                    CreatedDate=p.CreatedDate,
                };

                if (productPromote != null)
                {
                    data.PromotePrice = productPromote.TotalAmt;
                    data.PromotePercent = productPromote.Percent;
                }
                else
                {
                    data.PromotePrice = 0;
                    data.PromotePercent = 0;
                }

                productList.Add(data);
            }

            resp.Products = productList;
            return resp;
        }
    }
}