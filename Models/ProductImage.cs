using System;
using System.ComponentModel.DataAnnotations;

namespace MayMayShop.API.Models
{
    public partial class ProductImage
    {
        public int Id { get; set; }

        [StringLength(1000)]
        public string PublicId { get; set; }

        [StringLength(1000)]
        public string Url { get; set; }

        [StringLength(1000)]
        public string ThumbnailPublicId { get; set; }

        [StringLength(1000)]
        public string ThumbnailUrl { get; set; }

        public bool isMain { get; set; }

        public int ProductId { get; set; }
        public int? SeqNo { get; set; }

        public Product Product { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
