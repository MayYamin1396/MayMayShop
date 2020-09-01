using System;

namespace MayMayShop.API.Models
{
    public class PaymentService
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImgPath { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }
    }
}