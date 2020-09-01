using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }

        public int? SubCategoryId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public string Url { get; set; }
        public string VideoUrl {get;set;}
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}