using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [StringLength(10)]
        public string Code { get; set; }
        public int? ProductTypeId {get;set;}
        public virtual ProductType ProductType{get;set;}
        public int? BrandId {get;set;}
        public virtual Brand Brand {get;set;}
        public string Description { get; set; }
        public string ProductStatus {get;set;}

        public bool IsActive { get; set; }

        public int ProductCategoryId { get; set; }
        
        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductPromotion ProductPromotion { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }

        public virtual ICollection<ProductPrice> ProductPrice { get; set; }

    }
}