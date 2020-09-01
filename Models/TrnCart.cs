using System;

namespace MayMayShop.API.Models
{
    public class TrnCart
    {
        public int ProductId { get; set; }

        public int SkuId { get; set; }

        public int Qty { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}