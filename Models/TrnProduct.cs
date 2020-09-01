using System;

namespace MayMayShop.API.Models
{
    public class TrnProduct
    {
        public Guid Id { get; set; }

        public int ProductCategoryId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}