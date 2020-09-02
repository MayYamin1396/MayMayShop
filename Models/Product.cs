using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class Product
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int ProductCategoryId { get; set; }
        
        public DateTime? CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        //new field
        public int? BrandId {get;set;}

        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductPromotion ProductPromotion { get; set; }
        public virtual ICollection<ProductImage> ProductImage { get; set; }

        public virtual ICollection<ProductPrice> ProductPrice { get; set; }
        public virtual Brand Brand { get; set; }

    }
}